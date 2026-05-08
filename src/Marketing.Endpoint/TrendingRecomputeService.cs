using Marketing.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Marketing.Endpoint;

// Periodically recomputes Marketing.Product.Trending from the
// OrderActivity append log. This is the only writer of Trending -
// OrderPlacedHandler used to bump it on every event but that
// raced this recompute, so we let the recompute own the value
// and accept up-to-one-interval staleness as the trade.
//
// One pass:
//   trending[productId] = SUM(OrderActivity.Quantity)
//                         WHERE OccurredAt >= now - Window
//
// Five-minute interval keeps the value fresh enough for a demo
// without being so aggressive that you can see SQLite churn.
internal sealed class TrendingRecomputeService(
    IServiceProvider services,
    ILogger<TrendingRecomputeService> logger) : BackgroundService
{
    static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
    static readonly TimeSpan Window = TimeSpan.FromDays(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // First pass right after startup so any seeded events that
        // happen to be older than the window get their Trending zeroed
        // out without waiting a full Interval.
        await RecomputeAsync(stoppingToken);

        using var timer = new PeriodicTimer(Interval);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await timer.WaitForNextTickAsync(stoppingToken))
                    return;
                await RecomputeAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            // Narrow the safety net to the transient/operator errors
            // we're prepared to retry on the next tick. Anything else
            // (programmer bugs, disposed contexts, OOM, etc.) bubbles
            // out so the hosting infrastructure can crash the process
            // and surface the real problem.
            catch (Exception ex) when (ex is DbUpdateException or SqliteException)
            {
                logger.LogError(ex, "Trending recompute failed; will retry on the next tick");
            }
        }
    }

    async Task RecomputeAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MarketingDbContext>();

        var cutoff = DateTime.UtcNow - Window;

        var totals = await db.OrderActivity
            .Where(a => a.OccurredAt >= cutoff)
            .GroupBy(a => a.ProductId)
            .Select(g => new { ProductId = g.Key, Score = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Score, cancellationToken);

        var products = await db.Products.ToListAsync(cancellationToken);
        foreach (var product in products)
            product.Trending = totals.GetValueOrDefault(product.ProductId, 0);

        await db.SaveChangesAsync(cancellationToken);
    }
}
