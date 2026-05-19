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
        // Pull each cart-line product together with its live in-stock
        // count (sum of the InventoryDelta ledger, same calculation
        // ProductsComposer uses). The frontend caps its + button on
        // this so the customer can't queue more units than exist.
        var productRows = await dbContext.Products
            .Where(s => productIds.Contains(s.ProductId))
            .Select(p => new
            {
                Product = p,
                InStock = dbContext.InventoryDeltas
                    .Where(d => d.ProductId == p.ProductId)
                    .Sum(d => (int?)d.Delta) ?? 0
            })
            .ToListAsync(ct);
        var productLookup = productRows.ToDictionary(
            x => x.Product.ProductId,
            x => (x.Product, x.InStock));
        var orderedProducts = Mapper.MapToDictionary(cart, productLookup);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new CartLoaded
        {
            OrderedProducts = orderedProducts
        });

        vm.OrderId = orderId;
        vm.CartItems = orderedProducts.Values.ToList();
    }
}
