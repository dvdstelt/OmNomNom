using Shipping.Endpoint.Messages.Commands;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Workflow;

// Plain-data shipping address slice. Not Shipping.Data.Models.Address
// (an EF entity); slices are POCOs the framework (de)serializes as
// JSON. Translated into SubmitShippingAddress at workflow submit.
public sealed record ShippingAddressSlice(
    string FullName,
    string Street,
    string ZipCode,
    string Town,
    string Country);

public class ShippingAddressWorkflowSlice : WorkflowSlice<ShippingAddressSlice>
{
    public const string Key = "Shipping.Address";

    public override string SliceKey => Key;

    protected override object? BuildSubmitCommand(Guid orderId, ShippingAddressSlice slice) =>
        new SubmitShippingAddress
        {
            OrderId = orderId,
            FullName = slice.FullName,
            Street = slice.Street,
            ZipCode = slice.ZipCode,
            Town = slice.Town,
            Country = slice.Country
        };
}
