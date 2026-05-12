namespace Catalog.ServiceComposition.Workflow;

// Wire shape of one line item in the POST /cart/{orderId} JSON array.
// Translated into a CartSlice.CartLine by OrderSubmitComposer. Kept
// separate from CartLine so the wire format and the slice's internal
// shape can evolve independently.
public class CartItemForm
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
