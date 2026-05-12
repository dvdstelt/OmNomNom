namespace Finance.ServiceComposition.Workflow;

// Wire shape of the POST /buy/address/{orderId} body that Finance
// listens to. The frontend wraps the address in a billingAddress
// envelope: { billingAddress: { fullName, street, ... } }. The
// envelope exists only for the wire; internally we hand the
// BillingAddressData straight into BillingAddressSlice.
public class BillingAddressForm
{
    public BillingAddressData BillingAddress { get; set; } = null!;
}
