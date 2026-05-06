namespace Catalog.ServiceComposition.Events;

// Lean variant of CartLoaded raised by the side-summary widget on the
// address/payment routes. Carries only the (ProductId, Quantity) pairs;
// downstream subscribers (e.g. Finance) attach price/discount so the
// `OrderSummaryCard` can render Items / Discount / Total.
public class CartSummaryLoaded
{
    public Guid OrderId { get; set; }
    public IDictionary<Guid, dynamic> OrderedProducts { get; set; }
}
