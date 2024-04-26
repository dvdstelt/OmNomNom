namespace Shipping.ServiceComposition.Events;

public class DeliverySummaryLoaded
{
    public Guid DeliveryOptionId { get; set; }
    public dynamic DeliveryOption { get; set; }
}