using Catalog.Endpoint.Messages.Commands;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Workflow;

// Trigger slice that finalizes the workflow. Written by
// SummarySubmitComposer at submit time with the cart's contents
// copied in, so this single command carries both the line items
// (formerly SubmitOrderItems) and the "complete it" trigger.
// SubmitOrder is set high so the framework queues this command
// after all per-boundary writes have been queued.
public sealed record CompleteOrderSlice(IReadOnlyList<CartLine> Items)
{
    public static CompleteOrderSlice Empty { get; } = new([]);
}

public class CompleteOrderWorkflowSlice : WorkflowSlice<CompleteOrderSlice>
{
    public const string Key = "Catalog.CompleteOrder";

    public override string SliceKey => Key;

    public override int SubmitOrder => int.MaxValue;

    protected override object? BuildSubmitCommand(Guid orderId, CompleteOrderSlice slice)
    {
        if (slice.Items.Count == 0) return null;

        return new CompleteOrder
        {
            OrderId = orderId,
            Items = slice.Items
                .Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                })
                .ToList()
        };
    }
}
