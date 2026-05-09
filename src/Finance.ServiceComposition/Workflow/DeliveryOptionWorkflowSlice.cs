using Finance.Endpoint.Messages.Commands;
using WorkflowComposer;

namespace Finance.ServiceComposition.Workflow;

// Finance also tracks the delivery option choice because its pricing
// depends on it. Shipping owns the canonical delivery option list and
// will have its own slice; Finance only needs the chosen id.
public sealed record DeliveryOptionSlice(Guid DeliveryOptionId);

public class DeliveryOptionWorkflowSlice : WorkflowSlice<DeliveryOptionSlice>
{
    public const string Key = "Finance.DeliveryOption";

    public override string SliceKey => Key;

    protected override object? BuildSubmitCommand(Guid orderId, DeliveryOptionSlice slice) =>
        new SubmitDeliveryOption
        {
            OrderId = orderId,
            DeliveryOptionId = slice.DeliveryOptionId
        };
}
