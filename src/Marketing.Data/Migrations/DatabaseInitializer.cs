using LiteDB;
using Marketing.Data.Models;

namespace Marketing.Data.Migrations;

public static class DatabaseInitializer
{
    public static void Initialize(LiteDatabase context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var productsCollection = context.GetCollection<Product>();
        if (productsCollection.Count() > 0)
            return;

        productsCollection.InsertBulk(SeedData.Products());
    }
}