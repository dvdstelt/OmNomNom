using Finance.Data.Models;

namespace Finance.Data.Seed;

public static class SeedData
{
    static readonly Guid FremontId = Guid.Parse("9ea20ee6-d6bd-4654-80fb-3dd7e52792f9");
    static readonly Guid MoersleutelId = Guid.Parse("0275aa6d-fc48-4703-ac53-78a135c4a476");
    static readonly Guid BarcodeBlackYellowId = Guid.Parse("79f65fef-c186-41a2-80aa-f77bbd0ba173");
    static readonly Guid ExtravagantChocolateId = Guid.Parse("d504d1d1-071e-44a2-b9d3-2574fae14ed9");
    static readonly Guid Indulgence16Id = Guid.Parse("4a8b5587-f8a1-4ad9-a39d-e66d933cdea0");
    static readonly Guid Indulgence28Id = Guid.Parse("2080d73a-a726-4717-8f4b-7496a1957c37");
    static readonly Guid KbbsId = Guid.Parse("eebc3bcd-d045-4c99-962b-36130df0d9d4");
    static readonly Guid AbraxasId = Guid.Parse("b3461e6c-a731-4ceb-aa88-ccf12d4c417c"); //4.7
    static readonly Guid BourbonCountyId = Guid.Parse("c2ca906f-45a2-4cb7-8cc0-bfeb648197d6"); //4.4

    // IPA / Pale Ale
    static readonly Guid WhiteDogId = Guid.Parse("c646a449-05d7-45b3-9d6e-ec6438bc6f67");
    static readonly Guid TwentyTwoId = Guid.Parse("155793d1-71db-4d8f-9417-ecb917fb57a3"); //4.5
    static readonly Guid SusanId = Guid.Parse("2c29be7f-8210-4533-84c6-2afa0f5e9678"); //4.5
    static readonly Guid GalaxyFortPointId = Guid.Parse("d9193f6e-e99c-4bc6-a1ed-e9ee7cc1ff00");
    static readonly Guid MosaicFortPointId = Guid.Parse("25a008d8-0973-44c8-b39b-fc4436843f2e");
    static readonly Guid PseudoSueId = Guid.Parse("c38d7784-78e7-4326-a910-046ab80e63cc");
    static readonly Guid ChickenBroccoliId = Guid.Parse("976482a4-e6cd-4d2f-ae99-350d9af7c8d0");
    static readonly Guid TripleFruitBombId = Guid.Parse("16004eed-7468-4919-90e6-cc04427280f6");
    static readonly Guid JuiceLandV4Id = Guid.Parse("155e8d38-e60e-441f-b440-debabc30a02a");
    static readonly Guid DoubleMosaicDreamId = Guid.Parse("67f8323c-fc40-49d7-adf3-58199164c12e");

    // Sours
    static readonly Guid OudeGeuzeId = Guid.Parse("6f9d5730-5195-44f0-968e-829afb69a2dc"); // 4
    static readonly Guid TiarnaId = Guid.Parse("9d081a68-2cd0-481a-8ee5-6e99dfa927f1"); //4.1
    static readonly Guid BlueBerryMuffinId = Guid.Parse("6bd65199-e702-4411-b71d-65c856d879f4"); //4.2
    static readonly Guid CoastalSunshineId = Guid.Parse("d8bb0052-10de-45f7-8f40-97e004ecb372");

    // Mead
    static readonly Guid SunflowersId = Guid.Parse("0d397cfa-a38a-4696-9ddc-11b8a3501253");
    static readonly Guid BlueSuedeShewsId = Guid.Parse("f1da7f8d-03bf-4b60-adde-1591caeb58e2");

    // Quadrupel
    static readonly Guid LaTrappeQuad50Id = Guid.Parse("4ebb9ed5-f7cf-4e61-bc01-33248c3f6a4d");

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
            new() { ProductId = BarcodeBlackYellowId, Price = 32m, Discount = 0 },
            new() { ProductId = WhiteDogId, Price = 6, Discount = 0 },
            new() { ProductId = AbraxasId, Price = 40m, Discount = 0 },
            new() { ProductId = BourbonCountyId, Price = 25m, Discount = 0 },
            new() { ProductId = TwentyTwoId, Price = 11m, Discount = 0 },
            new() { ProductId = SusanId, Price = 9m, Discount = 10 },
            new() { ProductId = TiarnaId, Price = 12m, Discount = 0 },
            new() { ProductId = BlueBerryMuffinId, Price = 8m, Discount = 20 },
            new() { ProductId = OudeGeuzeId, Price = 16m, Discount = 0 },

            new() { ProductId = ExtravagantChocolateId, Price = 9.50m, Discount = 0 },
            new() { ProductId = Indulgence16Id, Price = 26.95m, Discount = 0 },
            new() { ProductId = Indulgence28Id, Price = 29.95m, Discount = 0 },
            new() { ProductId = KbbsId, Price = 474.95m, Discount = 0 },
            new() { ProductId = SunflowersId, Price = 67.50m, Discount = 0 },
            new() { ProductId = BlueSuedeShewsId, Price = 579.95m, Discount = 0 },
            new() { ProductId = GalaxyFortPointId, Price = 25.90m, Discount = 0 },
            new() { ProductId = MosaicFortPointId, Price = 13.90m, Discount = 0 },
            new() { ProductId = PseudoSueId, Price = 8.75m, Discount = 0 },
            new() { ProductId = LaTrappeQuad50Id, Price = 34.95m, Discount = 0 },
            new() { ProductId = ChickenBroccoliId, Price = 4.92m, Discount = 0 },
            new() { ProductId = TripleFruitBombId, Price = 4.99m, Discount = 0 },
            new() { ProductId = CoastalSunshineId, Price = 9.99m, Discount = 0 },
            new() { ProductId = JuiceLandV4Id, Price = 13.98m, Discount = 0 },
            new() { ProductId = DoubleMosaicDreamId, Price = 9.99m, Discount = 0 },
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