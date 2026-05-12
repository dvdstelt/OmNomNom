using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.ShoppingCart;

public class ShoppingCartExistsComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpGet("/existingCart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
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
