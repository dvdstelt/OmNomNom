using Marketing.Data;
using Marketing.Data.Seed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Marketing.ServiceComposition;

// Marketing has no separate endpoint that runs migrations + seed at
// startup, so the gateway process owns it via this hosted service.
// IHostedService.StartAsync fires after DI is built but before the
// app starts handling requests, which is the right window.
public class MarketingDatabaseInitializer(IServiceProvider services) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MarketingDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
