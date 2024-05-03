using Shipping.Data.Models;

namespace Shipping.Data.Migrations;

public static class SeedData
{
    static readonly Guid StandardId = Guid.Parse("17779198-9ae4-419c-8373-65aa4c050cf7");
    static readonly Guid ExpeditedId = Guid.Parse("6e114fc9-aecf-4815-970f-fedbd6709847");
    static readonly Guid PriorityId = Guid.Parse("8008abc4-a60f-4217-9e2c-745aa9af7f2c");
    
    public static IEnumerable<DeliveryOption> DeliveryOptions()
    {
        return new List<DeliveryOption>
        {
            new() { DeliveryOptionId = StandardId, Name = "Standard shipping", Description = "7-10 business days" },
            new() { DeliveryOptionId = ExpeditedId, Name = "Expedited shipping", Description = "2-5 business days" },
            new() { DeliveryOptionId = PriorityId, Name = "Priority shipping", Description = "Next business day" },
        };
    }

    static readonly Guid DennisId = Guid.Parse("767bbf72-4f0d-4e05-8ed7-45cadcb603ee");
    static readonly Guid PhilId = Guid.Parse("7e993977-f958-494b-bbc2-de4db6380d4b");

    public static IEnumerable<Location> Locations()
    {
        return
        [
            new() { Id = DennisId, Name = "Dennis", Town = "Rotterdam", ZipCode = "3077 AA", Country = "The Netherlands" },
            new() { Id = PhilId, Name = "Phil", Town = "Perth", ZipCode = "6000", Country = "Australia" }
        ];
    }
}