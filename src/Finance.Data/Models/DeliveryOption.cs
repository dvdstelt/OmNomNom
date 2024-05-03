namespace Finance.Data.Models;

public class DeliveryOptionKey
{
    public Guid DeliveryOptionId { get; set; }
    public Guid LocationId { get; set; }
}

public class DeliveryOption
{
    public DeliveryOptionKey Id { get; set; }
    public decimal Price { get; set; }
}