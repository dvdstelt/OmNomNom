using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using Microsoft.AspNetCore.Http;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Cart;

// Companion to `CartSummaryComposer` on the Catalog side: receives
// `CartSummaryLoaded`, attaches per-item Price/Discount from Finance's
// current price view, and exposes the cart total. The shipping route uses
// the heavier `OnCartLoaded` / `CartLoaded` pair instead, because
// it also renders product names/images.
public class OnCartSummaryLoaded(FinanceDbContext dbContext) : ICompositionEventsHandler<CartSummaryLoaded>
{
    public async Task Handle(CartSummaryLoaded @event, HttpRequest request)
    {
        var productIds = @event.OrderedProducts.Keys.ToList();
        var prices = await dbContext.CurrentPricesAsync(productIds, request.HttpContext.RequestAborted);

        decimal totalPrice = 0;
        foreach (var product in @event.OrderedProducts)
        {
            var price = prices[product.Key];
            product.Value.PriceId = price.PriceId;
            product.Value.Price = price.Price;
            product.Value.Discount = price.Discount;

            totalPrice += price.EffectivePrice() * (int)product.Value.Quantity;
        }

        var vm = request.GetComposedResponseModel();
        vm.TotalCartPrice = totalPrice;
    }
}
