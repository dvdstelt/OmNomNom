namespace Shipping.Endpoint.Messages.Commands;

public class SubmitDeliveryOption
{
    public Guid OrderId { get; set; }
    public Guid DeliveryOptionId { get; set; }
}