using Finance.Endpoint.Messages.Commands;
using WorkflowComposer;
using OrderItem = Finance.Endpoint.Messages.Commands.OrderItem;

namespace Finance.ServiceComposition.Workflow;

// Finance's view of the cart line items. Same shape as Catalog's
// CartSlice but stored under a Finance-namespaced key because each
// service boundary owns its slice (and its outgoing command).
// Translated into Finance's SubmitOrderItems at workflow submit.
public sealed record OrderItemsSlice(IReadOnlyList<OrderItemLine> Items)
{
    public static OrderItemsSlice Empty { get; } = new([]);
}

public sealed record OrderItemLine(Guid ProductId, int Quantity);

public class OrderItemsWorkflowSlice : WorkflowSlice<OrderItemsSlice>
{
    public const string Key = "Finance.OrderItems";

    public override string SliceKey => Key;

    protected override IReadOnlyList<string> Validate(OrderItemsSlice slice)
    {
        var errors = new List<string>();
        if (slice.Items.Count == 0)
            errors.Add("Order items must contain at least one item.");
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

    protected override object? BuildSubmitCommand(Guid orderId, OrderItemsSlice slice) =>
        new SubmitOrderItems
        {
            OrderId = orderId,
            Items = slice.Items
                .Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    OrderedQuantity = i.Quantity
                })
                .ToList()
        };
}
