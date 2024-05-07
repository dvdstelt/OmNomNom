using Catalog.Data.Models;

namespace Catalog.Data.Migrations;

public class SeedData
{
    // Stouts
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
            new Product() { Name = "11th Anniversary Stout (2020) - Fremont Brewing", Description = "11th Anniversary Stout (2020) van Fremont Brewing uit Washington, Verenigde Staten is een Exclusive Blend of Barrel-Aged Imperial Stouts van 12,2%.", ImageUrl = "fremontstout2020.png", Category = "Stout" ,ProductId =  FremontId},
            new Product() { Name = "Copper & Wool - Moersleutel", Description = "This imperial stout is matured to perfection in Willet, Ruby Port and Carcavelos barrels. Infused with Coffee Collective's finest Colombian coffee to enhance and accentuate the fruity flavors of strong wines. Let yourself be carried away by the art of brewing.", ImageUrl = "moersleutel-copper-wool.png", Category = "Stout" ,ProductId = MoersleutelId },
            new Product() { Name = "Cloud - White Dog Brewery", Description = "A stunner of a new IPA - New England / Hazy from the stills at White Dog Brewery. Batch #6, hopped with Citra T90 and Citra Cryo.", ImageUrl = "whitedog-cloud.png", Category = "IPA" ,ProductId = WhiteDogId },
            new Product() { Name = "Barrel-Aged Abraxas", Description = "It’s like a fudge-drizzled churro with a little bit of chili heat that slowly builds as you sip. Smooth vanilla cools things down on the finish, while bakery flavors from the cinnamon linger on the taste buds.", ImageUrl = "abraxas.png", Category = "Stout" , ProductId = AbraxasId },
            new Product() { Name = "Bourbon County Stout", Description = "Bourbon County Original Stout is aged in a mix of freshly emptied bourbon barrels from Buffalo Trace, Heaven Hill, Four Roses and Wild Turkey distilleries.", ImageUrl = "bourboncounty.png", Category = "Stout", ProductId = BourbonCountyId },
            // Stout
            // API
            // Sour
            new Product() { Name = "3 Fonteinen Oude Geuze", Description = "Blend of 1, 2, and 3 year old lambics. A true Geuze - a blend of 1, 2, and 3 year-old lambic, unfiltered and unpasteurized, and aged in the bottle for at least a year after blending. Refermentation in the bottle gives this Geuze its famous champagne-like spritziness. The lambic that goes into it is brewed only with 60% barley malt, 40% unmalted wheat, aged hops, and water, spontaneously fermented by wild yeasts, and matured in oak casks", ImageUrl = "oudegeuze.png", Category = "Sour", ProductId = OudeGeuzeId },
            new Product() { Name = "Allagash Tiarna 2013", Description = "Tiarna is a blend of two beers, one aged in oak and fermented with 100% brettanomyces and the other fermented in stainless with a blend of two belgian yeast strains. Both beers were brewed with a combination of 2 row and wheat malt in addition to specialty grains. It was hopped with Hallertau, Styrian Goldings and Cascade hops. The finished beer is dark golden in color with citrus, pineapple and bread in the aroma. The flavor of this tart beer has notes of grapefruit, lemon, and bread crust, and a long, dry finish.", ImageUrl = "tiarna.png", Category = "Sour", ProductId = TiarnaId },
            new Product() { Name = "Blueberry Muffin", Description = "What happens when you take our OG Blueberry Muffin recipe and smoothie-fy it?! Blueberry Muffin Fruit in the Can! We took the same tart beer you've come to know and love and made it thiccc with tons of fresh blueberry puree added post fermentation.", ImageUrl = "blueberrymuffin.png", Category = "Sour", ProductId = BlueBerryMuffinId },
            new Product() { Name = "Curiosity Twenty Two", Description = "At Tree House, we often brew following our moods, emotions, and fleeting moments of inspiration. This particular batch is inspired by a several things - a fresh lot of Amarillo hops, a renewed sense of focus for coaxing character out of our house yeast strain, and a wonderful piece of artwork by our friend Dean McKeever. Twenty Two pours a hazy orange in the glass and emits aromas of peach, apricot, and hints of papaya. The flavor follows suit, with peach being the predominant characteristic we experience. A firm orange rind bitterness balances things out. The Curiosity beers have always been brewed on a relatively small scale - we are excited that this particular batch is large, and more people than ever will get to try some. We hope you like it!", ImageUrl = "curiositytwentytwo.png", Category = "IPA", ProductId = TwentyTwoId },
            new Product() { Name = "Susan", Description = "Susan (1891-1976) was our grandfather’s sister. In her honor, we offer this version of an American IPA brewed with hops from the Yakima Valley and Riwaka hops from New Zealand.", ImageUrl = "susan.png", Category = "IPA", ProductId = SusanId },
        };
    }

    public static IEnumerable<InventoryDelta> ProductInventory()
    {
        return new List<InventoryDelta>
        {
            new() { ProductId = FremontId, Delta = 8, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = MoersleutelId, Delta = 6, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = WhiteDogId, Delta = 11, TimeStamp = DateTime.UtcNow, },

            new() { ProductId = AbraxasId, Delta = 4, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = BourbonCountyId, Delta = 6, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = TwentyTwoId, Delta = 22, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = SusanId, Delta = 19, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = OudeGeuzeId, Delta = 43, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = TiarnaId, Delta = 28, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = BlueBerryMuffinId, Delta = 36, TimeStamp = DateTime.UtcNow, },

        };
    }

    public static IEnumerable<InventorySnapshot> InventorySnapshots()
    {
        return new List<InventorySnapshot>
        {
            new() { ProductId = FremontId, EstimatedInStock = 10 },
            new() { ProductId = MoersleutelId, EstimatedInStock = 8 },
            new() { ProductId = WhiteDogId, EstimatedInStock = 20 },
            new() { ProductId = AbraxasId, EstimatedInStock = 5 },
            new() { ProductId = BourbonCountyId, EstimatedInStock = 8 },
            new() { ProductId = TwentyTwoId, EstimatedInStock = 25 },
            new() { ProductId = SusanId, EstimatedInStock = 22 },
            new() { ProductId = OudeGeuzeId, EstimatedInStock = 45 },
            new() { ProductId = TiarnaId, EstimatedInStock = 30 },
            new() { ProductId = BlueBerryMuffinId, EstimatedInStock = 40 },
        };
    }

    public static Order Orders()
    {
        return new Order
        {
            OrderId = Guid.Parse("08bebbee-0e7e-4368-afab-74f4720f5f4e"),
            Products =
            [
                new() { Quantity = 2, ProductId = MoersleutelId },
                new() { Quantity = 1, ProductId = SusanId }
            ]
        };
    }
}