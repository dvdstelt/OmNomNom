using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.ShoppingCart;

[CompositionHandler]
public class ShoppingCartExistsComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpGet("/existingCart/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var vm = request.GetComposedResponseModel();
        var ct = request.HttpContext.RequestAborted;

        if (orderId == Guid.Empty)
        {
            vm.OrderId = Guid.NewGuid();
            vm.CartItems = 0;
            return;
        }

        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct)
                   ?? CartSlice.Empty;

        vm.OrderId = orderId;
        vm.ItemsInCart = cart.Items.Count;
    }
}
