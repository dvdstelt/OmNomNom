using Marketing.Data.Models;

namespace Marketing.Data.Migrations;

public static class SeedData
{
    static readonly Guid FremontId = Guid.Parse("9ea20ee6-d6bd-4654-80fb-3dd7e52792f9");
    static readonly Guid MoersleutelId = Guid.Parse("0275aa6d-fc48-4703-ac53-78a135c4a476");
    static readonly Guid WhiteDogId = Guid.Parse("c646a449-05d7-45b3-9d6e-ec6438bc6f67");
    static readonly Guid CocaColaId = Guid.Parse("6f9d5730-5195-44f0-968e-829afb69a2dc");
    static readonly Guid RedBullId = Guid.Parse("9d081a68-2cd0-481a-8ee5-6e99dfa927f1");
    static readonly Guid MountainDewId = Guid.Parse("6bd65199-e702-4411-b71d-65c856d879f4");

    public static IEnumerable<Product> Products()
    {
        return new List<Product>
        {
            new Product() { ProductId = FremontId, Stars = 4.6, ReviewCount = 1337 },
            new Product() { ProductId = MoersleutelId, Stars = 4.4, ReviewCount = 911 },
            new Product() { ProductId = WhiteDogId, Stars = 4.1, ReviewCount = 42 },
            new Product() { ProductId = CocaColaId, Stars = 3.5, ReviewCount = 4861 },
            new Product() { ProductId = RedBullId, Stars = 2.9, ReviewCount = 2654 },
            new Product() { ProductId = MountainDewId, Stars = 3.8, ReviewCount = 459 },
        };
    }
}