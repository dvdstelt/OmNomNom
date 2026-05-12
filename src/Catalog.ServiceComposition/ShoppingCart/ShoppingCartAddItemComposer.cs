using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.ShoppingCart;

[CompositionHandler]
public class ShoppingCartAddItemComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpPost("/cart/addproduct/{orderId}")]
    public async Task Handle(Guid orderId, [FromBody] ProductModelBody detail)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        if (orderId == Guid.Empty)
            orderId = Guid.NewGuid();

        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct)
                   ?? CartSlice.Empty;
        var updated = Upsert(cart, detail.Id, detail.Quantity);
        await workflow.Write(orderId, CartWorkflowSlice.Key, updated, ct);

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
    }

    static CartSlice Upsert(CartSlice cart, Guid productId, int delta)
    {
        var lines = cart.Items.ToList();
        var existing = lines.FindIndex(l => l.ProductId == productId);
        if (existing >= 0)
        {
            var newQty = lines[existing].Quantity + delta;
            if (newQty <= 0)
                lines.RemoveAt(existing);
            else
                lines[existing] = lines[existing] with { Quantity = newQty };
        }
        else if (delta > 0)
        {
            lines.Add(new CartLine(productId, delta));
        }
        return new CartSlice(lines);
    }

    public class ProductModelBody
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
    }
}
