using Catalog.Data.Models;
using LiteDB;

namespace Catalog.Data.Migrations;

public static class DatabaseInitializer
{
    public static void Initialize(LiteDatabase context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var customers = context.GetCollection<Product>();
        if (customers.Count() > 0)
            return;

        customers.InsertBulk(SeedData.Products());

        var inventory = context.GetCollection<InventoryDelta>();
        inventory.InsertBulk(SeedData.ProductInventory());

        var inventorySnapshots = context.GetCollection<InventorySnapshot>();
        inventorySnapshots.InsertBulk(SeedData.InventorySnapshots());
    }
}