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

    protected override IReadOnlyList<string> Validate(CompleteOrderSlice slice)
    {
        var errors = new List<string>();
        if (slice.Items.Count == 0)
            errors.Add("Cart must contain at least one item.");
        for (var i = 0; i < slice.Items.Count; i++)
        {
            var line = slice.Items[i];
            if (line.ProductId == Guid.Empty)
                errors.Add($"Item {i}: ProductId is required.");
            if (line.Quantity <= 0)
                errors.Add($"Item {i}: Quantity must be greater than zero.");
        }
        return errors;
    }

    protected override object? BuildSubmitCommand(Guid orderId, CompleteOrderSlice slice) =>
        new CompleteOrder
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
