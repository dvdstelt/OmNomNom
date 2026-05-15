using WorkflowComposer;

namespace Catalog.ServiceComposition.Workflow;

// In-flight cart contents. Stored in checkout.db via WorkflowComposer
// and read by the cart/summary composers for UI display. The items
// are copied into CompleteOrderSlice at submit time, so this slice
// itself emits no command - CompleteOrder carries the line items
// directly.
public sealed record CartSlice(IReadOnlyList<CartLine> Items)
{
    public static CartSlice Empty { get; } = new([]);
}

public sealed record CartLine(Guid ProductId, int Quantity);

public class CartWorkflowSlice : WorkflowSlice<CartSlice>
{
    public const string Key = "Catalog.Cart";

    public override string SliceKey => Key;

    protected override object? BuildSubmitCommand(Guid orderId, CartSlice slice) => null;
}
