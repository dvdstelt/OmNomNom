using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Cart;

// Companion to `CartSummaryComposer` on the Catalog side: receives
// `CartSummaryLoaded`, attaches per-item Price/Discount from Finance's
// Products view, and exposes the cart total. The shipping route uses
// the heavier `CartLoadedSubscriber`/`CartLoaded` pair instead, because
// it also renders product names/images.
public class CartSummaryLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsHandler<CartSummaryLoaded>
{
    public async Task Handle(CartSummaryLoaded @event, HttpRequest request)
    {
        var productIds = @event.OrderedProducts.Keys.ToList();
        var resultSet = await dbContext.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToListAsync(request.HttpContext.RequestAborted);

        decimal totalPrice = 0;
        foreach (var product in @event.OrderedProducts)
        {
            var matchingProduct = resultSet.Single(p => p.ProductId == product.Key);
            product.Value.Price = matchingProduct.Price;
            product.Value.Discount = matchingProduct.Discount;

            totalPrice += matchingProduct.EffectivePrice() * (int)product.Value.Quantity;
        }

        var vm = request.GetComposedResponseModel();
        vm.TotalCartPrice = totalPrice;
    }
}
