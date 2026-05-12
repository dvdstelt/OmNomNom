namespace Catalog.ServiceComposition.Workflow;

// Wire shape of the POST /cart/addproduct/{orderId} body. The frontend
// posts { id, quantity } where `id` is the product to add and
// `quantity` is the +/- delta to apply (negative to decrement, large
// negative to remove). Upserted into the CartSlice.
public class AddProductForm
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}
