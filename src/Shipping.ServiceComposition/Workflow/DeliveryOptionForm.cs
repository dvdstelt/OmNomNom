namespace Shipping.ServiceComposition.Workflow;

// Wire shape of the POST /buy/shipping/{orderId} body that Shipping
// listens to. Same shape as Finance's DeliveryOptionForm but kept
// per-boundary because each ServiceComposition project owns its own
// slice and its own wire types.
public class DeliveryOptionForm
{
    public Guid DeliveryOptionId { get; set; }
}
