using LiteDB;
using PaymentInfo.Data.Models;

namespace PaymentInfo.Data.Migrations;

public class DatabaseInitializer
{
    public static void Initialize(LiteDatabase context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var creditCards = context.GetCollection<CreditCard>();
        if (creditCards.Count() > 0)
            return;

        creditCards.InsertBulk(SeedData.Products());

        var orderCollection = context.GetCollection<Order>();
        orderCollection.Insert(SeedData.Orders());
    }

}