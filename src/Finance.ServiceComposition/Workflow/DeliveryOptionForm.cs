namespace Finance.ServiceComposition.Workflow;

// Wire shape of the POST /buy/shipping/{orderId} body for the Finance
// composer. The frontend posts { deliveryOptionId } when the user
// picks a delivery option on the shipping screen. Shipping has its
// own copy of this form for its own composer/slice pair.
public class DeliveryOptionForm
{
    public Guid DeliveryOptionId { get; set; }
}
