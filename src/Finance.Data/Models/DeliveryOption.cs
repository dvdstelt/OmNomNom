namespace Finance.Data.Models;

public class DeliveryOption
{
    public Guid DeliveryOptionId { get; set; }
    public decimal Price { get; set; }

    // Null means this option is never free, regardless of order size.
    public decimal? FreeShippingThreshold { get; set; }
}