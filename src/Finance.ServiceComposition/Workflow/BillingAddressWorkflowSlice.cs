using Finance.Endpoint.Messages.Commands;
using WorkflowComposer;

namespace Finance.ServiceComposition.Workflow;

// Plain-data billing address slice. Not Finance.Data.Models.Address
// (which is an EF entity); slices are POCOs the framework
// (de)serializes as JSON.
public sealed record BillingAddressSlice(
    string FullName,
    string Street,
    string ZipCode,
    string Town,
    string Country);

public class BillingAddressWorkflowSlice : WorkflowSlice<BillingAddressSlice>
{
    public const string Key = "Finance.BillingAddress";

    public override string SliceKey => Key;

    protected override object? BuildSubmitCommand(Guid orderId, BillingAddressSlice slice) =>
        new SubmitBillingAddress
        {
            OrderId = orderId,
            FullName = slice.FullName,
            Street = slice.Street,
            ZipCode = slice.ZipCode,
            Town = slice.Town,
            Country = slice.Country
        };
}
