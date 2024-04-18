namespace Shipping.ServiceComposition.Events;

public class DeliveryOptionsLoaded
{
    public IDictionary<Guid, dynamic> DeliveryOptions { get; set; }
}