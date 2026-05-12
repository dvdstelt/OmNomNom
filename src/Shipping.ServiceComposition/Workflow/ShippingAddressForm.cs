using Shipping.Data.Models;

namespace Shipping.ServiceComposition.Workflow;

// Wire shape of the POST /buy/address/{orderId} body that Shipping
// listens to. The frontend wraps the address in a shippingAddress
// envelope: { shippingAddress: { fullName, street, ... } }. The
// envelope only exists for the wire; AddressSubmitComposer copies
// the fields into ShippingAddressSlice.
public class ShippingAddressForm
{
    public Address ShippingAddress { get; set; } = null!;
}
