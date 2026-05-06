using Marketing.Data.Models;

namespace Marketing.Data.Seed;

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
        // OrderCount = all-time orders. Trending = same as OrderCount on
        // a fresh seed since every seeded activity row is by construction
        // within the 30-day window; the recompute will keep them in sync
        // as the demo runs and seeded events age past the cutoff.
        return new List<Product>
        {
            new Product { ProductId = FremontId,         Rating = 4.6, RatingCount = 1337, OrderCount = 18, Trending = 12 },
            new Product { ProductId = MoersleutelId,     Rating = 4.4, RatingCount = 911,  OrderCount = 22, Trending = 22 },
            new Product { ProductId = WhiteDogId,        Rating = 4.1, RatingCount = 42,   OrderCount = 9,  Trending = 8  },
            new Product { ProductId = AbraxasId,         Rating = 4.7, RatingCount = 458,  OrderCount = 14, Trending = 4  },
            new Product { ProductId = BourbonCountyId,   Rating = 4.4, RatingCount = 754,  OrderCount = 31, Trending = 7  },
            new Product { ProductId = TwentyTwoId,       Rating = 4.5, RatingCount = 473,  OrderCount = 12, Trending = 10 },
            new Product { ProductId = SusanId,           Rating = 4.5, RatingCount = 748,  OrderCount = 27, Trending = 19 },
            new Product { ProductId = OudeGeuzeId,       Rating = 4,   RatingCount = 134,  OrderCount = 6,  Trending = 6  },
            new Product { ProductId = TiarnaId,          Rating = 4.1, RatingCount = 642,  OrderCount = 11, Trending = 3  },
            new Product { ProductId = BlueBerryMuffinId, Rating = 4.2, RatingCount = 846,  OrderCount = 24, Trending = 16 },
        };
    }

    // OrderActivity rows mirror the seeded counters. Each product's
    // Trending count maps to recent rows (within the 30-day window),
    // and the OrderCount-minus-Trending difference maps to older rows
    // outside the window. Once the periodic recompute runs, Trending
    // converges on the count of in-window rows for that product.
    public static IEnumerable<OrderActivity> OrderActivity()
    {
        var now = DateTime.UtcNow;
        var recent = new[] { -1, -2, -4, -6, -9, -13, -18, -22, -26, -29 };
        var oldEnough = new[] { -45, -60, -75, -90 };

        IEnumerable<OrderActivity> Build(Guid productId, int trending, int total)
        {
            var rows = new List<OrderActivity>();
            for (var i = 0; i < trending; i++)
                rows.Add(new OrderActivity
                {
                    ProductId = productId,
                    Quantity = 1,
                    OccurredAt = now.AddDays(recent[i % recent.Length])
                });
            for (var i = 0; i < total - trending; i++)
                rows.Add(new OrderActivity
                {
                    ProductId = productId,
                    Quantity = 1,
                    OccurredAt = now.AddDays(oldEnough[i % oldEnough.Length])
                });
            return rows;
        }

        var perProduct = new (Guid, int Trending, int Total)[]
        {
            (FremontId,         12, 18),
            (MoersleutelId,     22, 22),
            (WhiteDogId,         8, 9),
            (AbraxasId,          4, 14),
            (BourbonCountyId,    7, 31),
            (TwentyTwoId,       10, 12),
            (SusanId,           19, 27),
            (OudeGeuzeId,        6, 6),
            (TiarnaId,           3, 11),
            (BlueBerryMuffinId, 16, 24),
        };

        return perProduct.SelectMany(p => Build(p.Item1, p.Trending, p.Total));
    }
}
