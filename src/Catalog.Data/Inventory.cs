using Catalog.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Data;

public static class Inventory
{
    public static Task<int> CurrentStockAsync(this DbSet<InventoryDelta> deltas, Guid productId, CancellationToken cancellationToken)
        => deltas.Where(d => d.ProductId == productId).SumAsync(d => d.Delta, cancellationToken);

    public static InventoryDelta Reserve(Guid productId, int quantity) => new()
    {
        ProductId = productId,
        Delta = -quantity,
        TimeStamp = DateTime.UtcNow
    };
}
