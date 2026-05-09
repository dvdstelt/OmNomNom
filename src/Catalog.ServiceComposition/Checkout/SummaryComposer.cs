using System.Dynamic;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Checkout;

public class SummaryComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct)
                   ?? CartSlice.Empty;

        var productsModel = MapToDictionary(cart);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new SummaryLoaded
        {
            OrderId = orderId,
            Products = productsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
        vm.Products = productsModel;
    }

    static IDictionary<Guid, dynamic> MapToDictionary(CartSlice cart)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var line in cart.Items)
        {
            dynamic vm = new ExpandoObject();
            vm.ProductId = line.ProductId;
            vm.Quantity = line.Quantity;

            productsViewModel[line.ProductId] = vm;
        }

        return productsViewModel;
    }
}
