using LiteDB;
using Shipping.Data.Models;

namespace Shipping.Data.Migrations;

public static class DatabaseInitializer
{
    public static void Initialize(LiteDatabase context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var deliveryOptions = context.GetCollection<DeliveryOption>();
        if (deliveryOptions.Count() > 0)
            return;

        deliveryOptions.InsertBulk(SeedData.DeliveryOptions());
    }
}