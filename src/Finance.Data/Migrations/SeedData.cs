using Finance.Data.Models;

namespace Catalog.Data.Migrations;

public static class SeedData
{
    static readonly Guid FremontId = Guid.Parse("9ea20ee6-d6bd-4654-80fb-3dd7e52792f9");
    static readonly Guid MoersleutelId = Guid.Parse("0275aa6d-fc48-4703-ac53-78a135c4a476");
    static readonly Guid WhiteDogId = Guid.Parse("c646a449-05d7-45b3-9d6e-ec6438bc6f67");
    static readonly Guid CocaColaId = Guid.Parse("6f9d5730-5195-44f0-968e-829afb69a2dc");
    static readonly Guid RedBullId = Guid.Parse("9d081a68-2cd0-481a-8ee5-6e99dfa927f1");
    static readonly Guid MountainDewId = Guid.Parse("6bd65199-e702-4411-b71d-65c856d879f4");

    static readonly Guid DeliveryOption1 = Guid.Parse("17779198-9ae4-419c-8373-65aa4c050cf7");
    static readonly Guid DeliveryOption2 = Guid.Parse("6e114fc9-aecf-4815-970f-fedbd6709847");
    static readonly Guid DeliveryOption3 = Guid.Parse("8008abc4-a60f-4217-9e2c-745aa9af7f2c");

    public static IEnumerable<Product> Products()
    {
        return new List<Product>
        {
            new() { ProductId = FremontId, Price = 35, Discount = 0 },
            new() { ProductId = MoersleutelId, Price = 12, Discount = 0 },
            new() { ProductId = WhiteDogId, Price = 6, Discount = 0 },
            new() { ProductId = CocaColaId, Price = 0.8m, Discount = 0 },
            new() { ProductId = RedBullId, Price = 2.49m, Discount = 0 },
            new() { ProductId = MountainDewId, Price = 3.99m, Discount = 0 },
        };
    }

    public static IEnumerable<DeliveryOption> DeliveryOptions()
    {
        return new List<DeliveryOption>
        {
            new() { DeliveryOptionId = DeliveryOption1, Price = 2 },
            new() { DeliveryOptionId = DeliveryOption2, Price = 6 },
            new() { DeliveryOptionId = DeliveryOption3, Price = 14 },
        };
    }
}