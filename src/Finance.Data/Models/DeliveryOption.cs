namespace Finance.Data.Models;

public class DeliveryOption
{
    public Guid DeliveryOptionId { get; set; }
    public Guid LocationId { get; set; }
    public decimal Price { get; set; }
}