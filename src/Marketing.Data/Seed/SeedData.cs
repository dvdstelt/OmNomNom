using Marketing.Data.Models;

namespace Marketing.Data.Seed;

public static class SeedData
{
    static readonly Guid FremontId = Guid.Parse("9ea20ee6-d6bd-4654-80fb-3dd7e52792f9");
    static readonly Guid MoersleutelId = Guid.Parse("0275aa6d-fc48-4703-ac53-78a135c4a476");
    static readonly Guid BarcodeBlackYellowId = Guid.Parse("79f65fef-c186-41a2-80aa-f77bbd0ba173"); // 4.5
    static readonly Guid ExtravagantChocolateId = Guid.Parse("d504d1d1-071e-44a2-b9d3-2574fae14ed9"); // 4.22
    static readonly Guid Indulgence16Id = Guid.Parse("4a8b5587-f8a1-4ad9-a39d-e66d933cdea0"); // 4.52
    static readonly Guid Indulgence28Id = Guid.Parse("2080d73a-a726-4717-8f4b-7496a1957c37"); // 4.57
    static readonly Guid KbbsId = Guid.Parse("eebc3bcd-d045-4c99-962b-36130df0d9d4"); // 4.8
    static readonly Guid AbraxasId = Guid.Parse("b3461e6c-a731-4ceb-aa88-ccf12d4c417c"); //4.7
    static readonly Guid BourbonCountyId = Guid.Parse("c2ca906f-45a2-4cb7-8cc0-bfeb648197d6"); //4.4

    // IPA / Pale Ale
    static readonly Guid WhiteDogId = Guid.Parse("c646a449-05d7-45b3-9d6e-ec6438bc6f67");
    static readonly Guid TwentyTwoId = Guid.Parse("155793d1-71db-4d8f-9417-ecb917fb57a3"); //4.5
    static readonly Guid SusanId = Guid.Parse("2c29be7f-8210-4533-84c6-2afa0f5e9678"); //4.5
    static readonly Guid GalaxyFortPointId = Guid.Parse("d9193f6e-e99c-4bc6-a1ed-e9ee7cc1ff00"); // 4.37
    static readonly Guid MosaicFortPointId = Guid.Parse("25a008d8-0973-44c8-b39b-fc4436843f2e"); // 4.34
    static readonly Guid PseudoSueId = Guid.Parse("c38d7784-78e7-4326-a910-046ab80e63cc"); // 4.3
    static readonly Guid ChickenBroccoliId = Guid.Parse("976482a4-e6cd-4d2f-ae99-350d9af7c8d0"); // 4.22
    static readonly Guid TripleFruitBombId = Guid.Parse("16004eed-7468-4919-90e6-cc04427280f6"); // 3.93
    static readonly Guid JuiceLandV4Id = Guid.Parse("155e8d38-e60e-441f-b440-debabc30a02a"); // 4.09
    static readonly Guid DoubleMosaicDreamId = Guid.Parse("67f8323c-fc40-49d7-adf3-58199164c12e"); // 4.36

    // Sours
    static readonly Guid OudeGeuzeId = Guid.Parse("6f9d5730-5195-44f0-968e-829afb69a2dc"); // 4
    static readonly Guid TiarnaId = Guid.Parse("9d081a68-2cd0-481a-8ee5-6e99dfa927f1"); //4.1
    static readonly Guid BlueBerryMuffinId = Guid.Parse("6bd65199-e702-4411-b71d-65c856d879f4"); //4.2
    static readonly Guid CoastalSunshineId = Guid.Parse("d8bb0052-10de-45f7-8f40-97e004ecb372"); // 3.83

    // Mead
    static readonly Guid SunflowersId = Guid.Parse("0d397cfa-a38a-4696-9ddc-11b8a3501253"); // 4.64
    static readonly Guid BlueSuedeShewsId = Guid.Parse("f1da7f8d-03bf-4b60-adde-1591caeb58e2"); // 4.84

    // Quadrupel
    static readonly Guid LaTrappeQuad50Id = Guid.Parse("4ebb9ed5-f7cf-4e61-bc01-33248c3f6a4d"); // 4.46

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
            new Product { ProductId = BarcodeBlackYellowId, Rating = 4.5, RatingCount = 5204, OrderCount = 8,  Trending = 5  },
            new Product { ProductId = WhiteDogId,        Rating = 4.1, RatingCount = 42,   OrderCount = 9,  Trending = 8  },
            new Product { ProductId = AbraxasId,         Rating = 4.7, RatingCount = 458,  OrderCount = 14, Trending = 4  },
            new Product { ProductId = BourbonCountyId,   Rating = 4.4, RatingCount = 754,  OrderCount = 31, Trending = 7  },
            new Product { ProductId = TwentyTwoId,       Rating = 4.5, RatingCount = 473,  OrderCount = 12, Trending = 10 },
            new Product { ProductId = SusanId,           Rating = 4.5, RatingCount = 748,  OrderCount = 27, Trending = 19 },
            new Product { ProductId = OudeGeuzeId,       Rating = 4,   RatingCount = 134,  OrderCount = 6,  Trending = 6  },
            new Product { ProductId = TiarnaId,          Rating = 4.1, RatingCount = 642,  OrderCount = 11, Trending = 3  },
            new Product { ProductId = BlueBerryMuffinId, Rating = 4.2, RatingCount = 846,  OrderCount = 24, Trending = 16 },

            new Product { ProductId = ExtravagantChocolateId, Rating = 4.22, RatingCount = 3721,  OrderCount = 14, Trending = 6  },
            new Product { ProductId = Indulgence16Id,         Rating = 4.52, RatingCount = 2925,  OrderCount = 8,  Trending = 6  },
            new Product { ProductId = Indulgence28Id,         Rating = 4.57, RatingCount = 1934,  OrderCount = 9,  Trending = 7  },
            new Product { ProductId = KbbsId,                 Rating = 4.8,  RatingCount = 460,   OrderCount = 3,  Trending = 2  },
            new Product { ProductId = SunflowersId,           Rating = 4.64, RatingCount = 589,   OrderCount = 5,  Trending = 4  },
            new Product { ProductId = BlueSuedeShewsId,       Rating = 4.84, RatingCount = 3690,  OrderCount = 4,  Trending = 4  },
            new Product { ProductId = GalaxyFortPointId,      Rating = 4.37, RatingCount = 32715, OrderCount = 32, Trending = 14 },
            new Product { ProductId = MosaicFortPointId,      Rating = 4.34, RatingCount = 28747, OrderCount = 28, Trending = 11 },
            new Product { ProductId = PseudoSueId,            Rating = 4.3,  RatingCount = 55451, OrderCount = 41, Trending = 18 },
            new Product { ProductId = LaTrappeQuad50Id,       Rating = 4.46, RatingCount = 3195,  OrderCount = 13, Trending = 9  },
            new Product { ProductId = ChickenBroccoliId,      Rating = 4.22, RatingCount = 9744,  OrderCount = 22, Trending = 16 },
            new Product { ProductId = TripleFruitBombId,      Rating = 3.93, RatingCount = 2997,  OrderCount = 14, Trending = 8  },
            new Product { ProductId = JuiceLandV4Id,          Rating = 4.09, RatingCount = 3051,  OrderCount = 19, Trending = 13 },
            new Product { ProductId = DoubleMosaicDreamId,    Rating = 4.36, RatingCount = 48485, OrderCount = 38, Trending = 22 },
            new Product { ProductId = CoastalSunshineId,      Rating = 3.83, RatingCount = 254,   OrderCount = 4,  Trending = 3  },
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
            (BarcodeBlackYellowId, 5, 8),
            (WhiteDogId,         8, 9),
            (AbraxasId,          4, 14),
            (BourbonCountyId,    7, 31),
            (TwentyTwoId,       10, 12),
            (SusanId,           19, 27),
            (OudeGeuzeId,        6, 6),
            (TiarnaId,           3, 11),
            (BlueBerryMuffinId, 16, 24),

            (ExtravagantChocolateId,  6, 14),
            (Indulgence16Id,          6,  8),
            (Indulgence28Id,          7,  9),
            (KbbsId,                  2,  3),
            (SunflowersId,            4,  5),
            (BlueSuedeShewsId,        4,  4),
            (GalaxyFortPointId,      14, 32),
            (MosaicFortPointId,      11, 28),
            (PseudoSueId,            18, 41),
            (LaTrappeQuad50Id,        9, 13),
            (ChickenBroccoliId,      16, 22),
            (TripleFruitBombId,       8, 14),
            (JuiceLandV4Id,          13, 19),
            (DoubleMosaicDreamId,    22, 38),
            (CoastalSunshineId,       3,  4),
        };

        return perProduct.SelectMany(p => Build(p.Item1, p.Trending, p.Total));
    }
}
