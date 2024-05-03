namespace Shipping.Endpoint.Messages.Events;

public class OrderShipped
{
    public Guid OrderId { get; set; }
}