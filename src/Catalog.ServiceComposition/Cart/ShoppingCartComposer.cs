using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Cart;

[CompositionHandler]
public class ShoppingCartComposer(IWorkflowStore workflow, CatalogDbContext dbContext, IHttpContextAccessor http)
{
    [HttpGet("/cart/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var vm = request.GetComposedResponseModel();
        var ct = request.HttpContext.RequestAborted;

        // Returning an empty cart on a missing slice silently hides the
        // fact that WorkflowCleanupHostedService reaped what the
        // customer had. 410 lets the SPA show an explicit
        // "your cart was cleared" view and reset the orderId so the
        // next interaction starts fresh.
        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct);
        if (cart is null)
        {
            request.SetActionResult(new StatusCodeResult(StatusCodes.Status410Gone));
            return;
        }

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
