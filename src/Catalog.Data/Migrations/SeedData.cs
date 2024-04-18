using Catalog.Data.Models;

namespace Catalog.Data.Migrations;

public class SeedData
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
            new Product() { Name = "11th Anniversary Stout (2020) - Fremont Brewing", Description = "11th Anniversary Stout (2020) van Fremont Brewing uit Washington, Verenigde Staten is een Exclusive Blend of Barrel-Aged Imperial Stouts van 12,2%.", ImageUrl = "fremontstout2020.png", Category = "Craft Beer" ,ProductId =  FremontId},
            new Product() { Name = "Copper & Wool - Moersleutel", Description = "This imperial stout is matured to perfection in Willet, Ruby Port and Carcavelos barrels. Infused with Coffee Collective's finest Colombian coffee to enhance and accentuate the fruity flavors of strong wines. Let yourself be carried away by the art of brewing.", ImageUrl = "moersleutel-copper-wool.png", Category = "Craft Beer" ,ProductId = MoersleutelId },
            new Product() { Name = "Cloud - White Dog Brewery", Description = "A stunner of a new IPA - New England / Hazy from the stills at White Dog Brewery. Batch #6, hopped with Citra T90 and Citra Cryo.", ImageUrl = "whitedog-cloud.png", Category = "Craft Beer" ,ProductId = WhiteDogId },
            new Product() { Name = "Mountain Dew", Description = "Mountain Dew is an originally American soft drink with a delicious citrus flavor. It is thirst-quenching and super refreshing for any time of the day.", ImageUrl = "mountain-dew.png", Category = "Soda" ,ProductId = MountainDewId },
            new Product() { Name = "Red Bull", Description = "Red Bull Energy Drink gives you Wiiings whenever you need them.", ImageUrl = "red-bull.png", Category = "Soda" ,ProductId = RedBullId },
            new Product() { Name = "Coca Cola", Description = "Coca Cola regular is the most famous soft drink in the world. Since 1886, the secret recipe has ensured the authentic taste. The taste has never changed (and never equaled).", ImageUrl = "coca-cola.png", Category = "Soda" ,ProductId = CocaColaId },
        };
    }

    public static IEnumerable<InventoryDeltas> ProductInventory()
    {
        return new List<InventoryDeltas>
        {
            new() { ProductId = FremontId, Delta = 8, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = MoersleutelId, Delta = 6, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = WhiteDogId, Delta = 11, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = MountainDewId, Delta = 28, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = RedBullId, Delta = 36, TimeStamp = DateTime.UtcNow, },
            new() { ProductId = CocaColaId, Delta = 167, TimeStamp = DateTime.UtcNow, },
        };
    }

    public static IEnumerable<InventorySnapshot> InventorySnapshots()
    {
        return new List<InventorySnapshot>
        {
            new() { ProductId = FremontId, EstimatedInStock = 10 },
            new() { ProductId = MoersleutelId, EstimatedInStock = 8 },
            new() { ProductId = WhiteDogId, EstimatedInStock = 20 },
            new() { ProductId = MountainDewId, EstimatedInStock = 35 },
            new() { ProductId = RedBullId, EstimatedInStock = 50 },
            new() { ProductId = CocaColaId, EstimatedInStock = 200 },
        };
    }
}