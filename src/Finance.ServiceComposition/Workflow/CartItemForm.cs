namespace Finance.ServiceComposition.Workflow;

// Finance's view of one line item in the POST /cart/{orderId} JSON
// array. Same shape as Catalog's CartItemForm but kept per-boundary
// because each ServiceComposition project owns its own slice and its
// own wire types.
public class CartItemForm
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
