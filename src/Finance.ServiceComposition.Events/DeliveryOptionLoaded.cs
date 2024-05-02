namespace Finance.ServiceComposition.Events;

public class DeliveryOptionLoaded
{
    public Guid DeliveryOptionId { get; set; }
    public dynamic DeliveryOption { get; set; }
}