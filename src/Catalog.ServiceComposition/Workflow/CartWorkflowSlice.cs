using Catalog.Endpoint.Messages.Commands;
using WorkflowComposer;
using OrderItem = Catalog.Endpoint.Messages.Commands.OrderItem;

namespace Catalog.ServiceComposition.Workflow;

// In-flight cart contents. Stored in checkout.db via WorkflowComposer.
// Translated into SubmitOrderItems at workflow submit time.
public sealed record CartSlice(IReadOnlyList<CartLine> Items)
{
    public static CartSlice Empty { get; } = new([]);
}

public sealed record CartLine(Guid ProductId, int Quantity);

public class CartWorkflowSlice : WorkflowSlice<CartSlice>
{
    public const string Key = "Catalog.Cart";

    public override string SliceKey => Key;

    protected override object? BuildSubmitCommand(Guid orderId, CartSlice slice)
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
