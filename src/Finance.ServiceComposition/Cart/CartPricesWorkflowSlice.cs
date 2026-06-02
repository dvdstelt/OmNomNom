using WorkflowComposer;

namespace Finance.ServiceComposition.Cart;

// The PriceId the customer saw, captured per product as items are added
// to the in-flight cart. Finance-owned and Finance-keyed: it lives
// alongside Catalog's own CartSlice under the same orderId but is never
// read or written by Catalog. OrderSubmitComposer joins each submitted
// line to this slice to lock the price in. Capture-only, so it emits no
// command of its own.
public sealed record CartPricesSlice(IReadOnlyList<CartPriceLine> Items)
{
    public static CartPricesSlice Empty { get; } = new([]);
}

public sealed record CartPriceLine(Guid ProductId, Guid PriceId);

public class CartPricesWorkflowSlice : WorkflowSlice<CartPricesSlice>
{
    public const string Key = "Finance.CartPrices";

    public override string SliceKey => Key;

    protected override object? BuildSubmitCommand(Guid orderId, CartPricesSlice slice) => null;
}
