namespace Finance.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public Address BillingAddress { get; set; } = new();
}