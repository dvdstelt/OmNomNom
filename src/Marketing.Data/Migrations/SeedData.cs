using Marketing.Data.Models;

namespace Marketing.Data.Migrations;

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

    public static IEnumerable<Product> Products()
    {
        return new List<Product>
        {
            new Product() { ProductId = FremontId, Stars = 4.6, ReviewCount = 1337 },
            new Product() { ProductId = MoersleutelId, Stars = 4.4, ReviewCount = 911 },
            new Product() { ProductId = WhiteDogId, Stars = 4.1, ReviewCount = 42 },
            new Product() { ProductId = AbraxasId, Stars = 4.7, ReviewCount = 458 },
            new Product() { ProductId = BourbonCountyId, Stars = 4.4, ReviewCount = 754 },
            new Product() { ProductId = TwentyTwoId, Stars = 4.5, ReviewCount = 473 },

            new Product() { ProductId = SusanId, Stars = 4.5, ReviewCount = 748 },
            new Product() { ProductId = OudeGeuzeId, Stars = 4, ReviewCount = 134 },
            new Product() { ProductId = TiarnaId, Stars = 4.1, ReviewCount = 642 },
            new Product() { ProductId = BlueBerryMuffinId, Stars = 4.2, ReviewCount = 846 },

        };
    }
}