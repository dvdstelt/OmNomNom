using Shipping.Endpoint.Messages.Commands;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Workflow;

// Shipping owns the canonical delivery option list (read from
// shipping.db) and stores the user's chosen id here. Finance has its
// own DeliveryOption slice for pricing concerns - same data, two
// slices, by the per-boundary autonomy convention.
public sealed record DeliveryOptionSlice(Guid DeliveryOptionId);

public class DeliveryOptionWorkflowSlice : WorkflowSlice<DeliveryOptionSlice>
{
    public const string Key = "Shipping.DeliveryOption";

    public override string SliceKey => Key;

    protected override IReadOnlyList<string> Validate(DeliveryOptionSlice slice) =>
        slice.DeliveryOptionId == Guid.Empty
            ? ["DeliveryOptionId is required."]
            : [];

    protected override object? BuildSubmitCommand(Guid orderId, DeliveryOptionSlice slice) =>
        new SubmitDeliveryOption
        {
            OrderId = orderId,
            DeliveryOptionId = slice.DeliveryOptionId
        };
}
