using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
[CompositionHandler]
public class ShippingScreenCartComposer(IWorkflowStore workflow, CatalogDbContext dbContext, IHttpContextAccessor http)
{
    [HttpGet("/buy/shipping/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        // No way to reach the shipping screen without a cart; treat a
        // missing slice as the WorkflowCleanupHostedService having
        // reaped it and 410 so the SPA can show the expired-session
        // view instead of an empty list.
        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct);
        if (cart is null)
        {
            request.SetActionResult(new StatusCodeResult(StatusCodes.Status410Gone));
            return;
        }

        // Items here aren't editable (the shipping page renders them as
        // a static list), but the Cart mapper now requires an InStock
        // figure per line — pass 0 since this surface doesn't need it.
        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var productLookup = (await dbContext.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync(ct))
            .ToDictionary(p => p.ProductId, p => (Product: p, InStock: 0));
        var orderedProducts = Cart.Mapper.MapToDictionary(cart, productLookup);

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
