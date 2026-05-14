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

    protected override IReadOnlyList<string> Validate(ShippingAddressSlice slice)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(slice.FullName))
            errors.Add("FullName is required.");
        if (string.IsNullOrWhiteSpace(slice.Street))
            errors.Add("Street is required.");
        if (string.IsNullOrWhiteSpace(slice.ZipCode))
            errors.Add("ZipCode is required.");
        if (string.IsNullOrWhiteSpace(slice.Town))
            errors.Add("Town is required.");
        if (string.IsNullOrWhiteSpace(slice.Country))
            errors.Add("Country is required.");
        return errors;
    }

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
