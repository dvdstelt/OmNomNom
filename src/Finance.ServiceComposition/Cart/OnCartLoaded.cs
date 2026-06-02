using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using Microsoft.AspNetCore.Http;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Cart;

public class OnCartLoaded(FinanceDbContext dbContext) : ICompositionEventsHandler<CartLoaded>
{
    public async Task Handle(CartLoaded @event, HttpRequest request)
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
