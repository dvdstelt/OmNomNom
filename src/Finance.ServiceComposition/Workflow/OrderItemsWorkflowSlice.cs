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

    protected override object? BuildSubmitCommand(Guid orderId, OrderItemsSlice slice)
    {
        if (slice.Items.Count == 0) return null;

        return new SubmitOrderItems
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
