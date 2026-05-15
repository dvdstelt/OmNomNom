namespace Shipping.Data.Models;

public class DeliveryOption
{
    public Guid DeliveryOptionId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}