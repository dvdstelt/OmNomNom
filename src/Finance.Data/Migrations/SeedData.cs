using Finance.Data.Models;

namespace Finance.Data.Migrations;

public static class SeedData
{
    static readonly Guid FremontId = Guid.Parse("9ea20ee6-d6bd-4654-80fb-3dd7e52792f9");
    static readonly Guid MoersleutelId = Guid.Parse("0275aa6d-fc48-4703-ac53-78a135c4a476");
    static readonly Guid AbraxasId = Guid.Parse("b3461e6c-a731-4ceb-aa88-ccf12d4c417c"); //4.7
    static readonly Guid BourbonCountyId = Guid.Parse("c2ca906f-45a2-4cb7-8cc0-bfeb648197d6"); //4.4

    // IPA
    static readonly Guid WhiteDogId = Guid.Parse("c646a449-05d7-45b3-9d6e-ec6438bc6f67");
    static readonly Guid TwentyTwoId = Guid.Parse("155793d1-71db-4d8f-9417-ecb917fb57a3"); //4.5
    static readonly Guid SusanId = Guid.Parse("2c29be7f-8210-4533-84c6-2afa0f5e9678"); //4.5

    // Sours
    static readonly Guid OudeGeuzeId = Guid.Parse("6f9d5730-5195-44f0-968e-829afb69a2dc"); // 4
    static readonly Guid TiarnaId = Guid.Parse("9d081a68-2cd0-481a-8ee5-6e99dfa927f1"); //4.1
    static readonly Guid BlueBerryMuffinId = Guid.Parse("6bd65199-e702-4411-b71d-65c856d879f4"); //4.2

    static readonly Guid StandardId = Guid.Parse("17779198-9ae4-419c-8373-65aa4c050cf7");
    static readonly Guid ExpeditedId = Guid.Parse("6e114fc9-aecf-4815-970f-fedbd6709847");
    static readonly Guid PriorityId = Guid.Parse("8008abc4-a60f-4217-9e2c-745aa9af7f2c");

    public static IEnumerable<Product> Products()
    {
        return new List<Product>
        {
            // PriceId can be random, as it's only used in orders and we don't seed orders
            new() { ProductId = FremontId, Price = 35, Discount = 0 },
            new() { ProductId = MoersleutelId, Price = 12, Discount = 0 },
            new() { ProductId = WhiteDogId, Price = 6, Discount = 0 },
            new() { ProductId = AbraxasId, Price = 40m, Discount = 0 },
            new() { ProductId = BourbonCountyId, Price = 25m, Discount = 0 },
            new() { ProductId = TwentyTwoId, Price = 11m, Discount = 0 },
            new() { ProductId = SusanId, Price = 9m, Discount = 0 },
            new() { ProductId = TiarnaId, Price = 12m, Discount = 0 },
            new() { ProductId = BlueBerryMuffinId, Price = 8m, Discount = 0 },
            new() { ProductId = OudeGeuzeId, Price = 16m, Discount = 0 },
        };
    }

    public static IEnumerable<DeliveryOption> DeliveryOptions()
    {
        return new List<DeliveryOption>
        {
            new() { DeliveryOptionId = StandardId, Price = 2 },
            new() { DeliveryOptionId = ExpeditedId, Price = 6 },
            new() { DeliveryOptionId = PriorityId, Price = 14 },
        };
    }

    public static Order Orders()
    {
        return new Order()
        {
            OrderId = Guid.Parse("08bebbee-0e7e-4368-afab-74f4720f5f4e"),
            DeliveryOptionId = ExpeditedId,
            BillingAddress = new()
            {
                FullName = "Dennis van der Stelt",
                Street = "Van Zandvlietplein 1",
                ZipCode = "3077 AA",
                Town = "Rotterdam",
                Country = "The Netherlands"
            },
            Items =
            [
                new() { ProductId = MoersleutelId, Price = 12m, Quantity = 2 },
                new() { ProductId = SusanId, Price = 9m, Quantity = 1 }
            ]
        };
    }
}