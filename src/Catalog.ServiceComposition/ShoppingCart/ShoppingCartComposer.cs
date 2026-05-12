using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.ShoppingCart;

public class ShoppingCartComposer(IWorkflowStore workflow, CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct)
                   ?? CartSlice.Empty;

        var productIds = cart.Items.Select(s => s.ProductId).ToList();
        var products = await dbContext.Products
            .Where(s => productIds.Contains(s.ProductId))
            .ToListAsync(ct);
        var orderedProducts = Mapper.MapToDictionary(cart, products);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new CartLoaded
        {
            OrderedProducts = orderedProducts
        });

        vm.OrderId = orderId;
        vm.CartItems = orderedProducts.Values.ToList();
    }
}
