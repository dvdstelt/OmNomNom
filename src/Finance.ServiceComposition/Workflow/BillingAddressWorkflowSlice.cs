using Finance.Endpoint.Messages.Commands;
using WorkflowComposer;

namespace Finance.ServiceComposition.Workflow;

// Internal billing address shape, shared by the slice, the
// composer's request-binding DTO, and view-models so we stop hand-
// copying five strings at every layer. NOT the wire format - that's
// SubmitBillingAddress, which keeps its own field-by-field copy so
// its shape stays a stable contract independent of internal
// refactors.
public sealed record BillingAddressData(
    string FullName,
    string Street,
    string ZipCode,
    string Town,
    string Country);

// Plain-data billing address slice. Not Finance.Data.Models.Address
// (which is an EF entity); slices are POCOs the framework
// (de)serializes as JSON.
public sealed record BillingAddressSlice(BillingAddressData Address);

public class BillingAddressWorkflowSlice : WorkflowSlice<BillingAddressSlice>
{
    public const string Key = "Finance.BillingAddress";

    public override string SliceKey => Key;

    protected override IReadOnlyList<string> Validate(BillingAddressSlice slice)
    {
        var errors = new List<string>();
        if (slice.Address is null)
        {
            errors.Add("Address is required.");
            return errors;
        }
        if (string.IsNullOrWhiteSpace(slice.Address.FullName))
            errors.Add("FullName is required.");
        if (string.IsNullOrWhiteSpace(slice.Address.Street))
            errors.Add("Street is required.");
        if (string.IsNullOrWhiteSpace(slice.Address.ZipCode))
            errors.Add("ZipCode is required.");
        if (string.IsNullOrWhiteSpace(slice.Address.Town))
            errors.Add("Town is required.");
        if (string.IsNullOrWhiteSpace(slice.Address.Country))
            errors.Add("Country is required.");
        return errors;
    }

    // Slice -> command is one of the two boundary copies we still
    // pay explicitly. SubmitBillingAddress intentionally doesn't
    // reference BillingAddressData so the wire contract can evolve
    // independently of the internal shape.
    protected override object? BuildSubmitCommand(Guid orderId, BillingAddressSlice slice) =>
        new SubmitBillingAddress
        {
            OrderId = orderId,
            FullName = slice.Address.FullName,
            Street = slice.Address.Street,
            ZipCode = slice.Address.ZipCode,
            Town = slice.Address.Town,
            Country = slice.Address.Country
        };
}
