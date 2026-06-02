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

    static readonly DateTimeOffset SeededAt = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    // One current price per product. PriceId is generated per seed run;
    // seeding only runs against an empty database and orders reference
    // these rows by PriceId within the same run, so stable ids across
    // runs aren't needed.
    static readonly IReadOnlyList<ProductPrice> ProductPriceSeed =
    [
        Price(FremontId, 35, 0),
        Price(MoersleutelId, 12, 0),
        Price(BarcodeBlackYellowId, 32m, 0),
        Price(WhiteDogId, 6, 0),
        Price(AbraxasId, 40m, 0),
        Price(BourbonCountyId, 25m, 0),
        Price(TwentyTwoId, 11m, 0),
        Price(SusanId, 9m, 10),
        Price(TiarnaId, 12m, 0),
        Price(BlueBerryMuffinId, 8m, 20),
        Price(OudeGeuzeId, 16m, 0),

        Price(ExtravagantChocolateId, 9.50m, 0),
        Price(Indulgence16Id, 26.95m, 0),
        Price(Indulgence28Id, 29.95m, 0),
        Price(KbbsId, 474.95m, 0),
        Price(SunflowersId, 67.50m, 0),
        Price(BlueSuedeShewsId, 579.95m, 0),
        Price(GalaxyFortPointId, 25.90m, 0),
        Price(MosaicFortPointId, 13.90m, 0),
        Price(PseudoSueId, 8.75m, 0),
        Price(LaTrappeQuad50Id, 34.95m, 0),
        Price(ChickenBroccoliId, 4.92m, 0),
        Price(TripleFruitBombId, 4.99m, 0),
        Price(CoastalSunshineId, 9.99m, 0),
        Price(JuiceLandV4Id, 13.98m, 0),
        Price(DoubleMosaicDreamId, 9.99m, 0),
    ];

    static ProductPrice Price(Guid productId, decimal price, decimal discount) =>
        new()
        {
            PriceId = Guid.NewGuid(),
            ProductId = productId,
            Price = price,
            Discount = discount,
            ValidFrom = SeededAt
        };

    public static IEnumerable<Product> Products() =>
        ProductPriceSeed.Select(p => new Product { ProductId = p.ProductId });

    public static IEnumerable<ProductPrice> ProductPrices() => ProductPriceSeed;

    public static IEnumerable<DeliveryOption> DeliveryOptions()
    {
        return new List<DeliveryOption>
        {
            new() { DeliveryOptionId = StandardId, Price = 2, FreeShippingThreshold = 50m },
            new() { DeliveryOptionId = ExpeditedId, Price = 6, FreeShippingThreshold = 75m },
            new() { DeliveryOptionId = PriorityId, Price = 14, FreeShippingThreshold = null },
        };
    }

    public static Order Orders()
    {
        return new Order
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
                LineFor(MoersleutelId, 2),
                LineFor(SusanId, 1)
            ]
        };
    }

    // A seeded order line snapshots Price/Discount from the locked price
    // and records the PriceId, exactly as SubmitOrderItemsHandler does.
    static OrderItem LineFor(Guid productId, int quantity)
    {
        var price = ProductPriceSeed.First(p => p.ProductId == productId);
        return new OrderItem
        {
            ProductId = productId,
            PriceId = price.PriceId,
            Price = price.Price,
            Discount = price.Discount,
            BillableQuantity = quantity
        };
    }
}