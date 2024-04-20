using Finance.Data.Models;
using LiteDB;

namespace Finance.Data.Migrations;

public static class DatabaseInitializer
{
    public static void Initialize(LiteDatabase context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var products = context.GetCollection<Product>();
        if (products.Count() > 0)
            return;

        products.InsertBulk(SeedData.Products());

        var deliveryOptions = context.GetCollection<DeliveryOption>();
        deliveryOptions.InsertBulk(SeedData.DeliveryOptions());
    }
}