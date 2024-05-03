namespace Finance.Endpoint.Messages.Commands;

public class SubmitOrderItems
{
    public Guid OrderId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public Guid PriceId { get; set; }
    public int Quantity { get; set; }
}