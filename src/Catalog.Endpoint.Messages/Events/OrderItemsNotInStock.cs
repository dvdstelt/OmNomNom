namespace Catalog.Endpoint.Messages.Events;

public class OrderItemsNotFulfilled
{
    public Guid OrderId { get; set; }
    public List<OrderItem> ItemsNotInStock { get; set; } = new();
    
    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public int QuantityWanted { get; set; }
        public int QuantityInStock { get; set; }
    }
}

