using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Checkout;

// Powers the shipping screen, which renders the full `CartItemList`
// (product image + name) on top of the order summary. Reads the cart
// slice from WorkflowComposer; product details are joined from the
// catalog domain DB.
//
// Address and payment use the leaner `CartSummaryComposer` instead.
public class ShoppingCartComposer(IWorkflowStore workflow, CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct)
                   ?? CartSlice.Empty;

        var products = await dbContext.Products.ToListAsync(ct);
        var orderedProducts = ShoppingCart.Mapper.MapToDictionary(cart, products);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new CartLoaded
        {
            OrderedProducts = orderedProducts
        });

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
        vm.CartItems = orderedProducts.Values.ToList();
    }
}
