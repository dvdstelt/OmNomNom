namespace Catalog.ServiceComposition.Events;

// Email-summary specific: subscribers can use the orderId to look up
// per-line state (e.g. Finance attaching Fulfilled) on top of the
// generic ProductsLoaded enrichment.
public class OrderEmailProductsLoaded
{
    public Guid OrderId { get; set; }
    public IDictionary<Guid, dynamic> Products { get; set; } = null!;
}
