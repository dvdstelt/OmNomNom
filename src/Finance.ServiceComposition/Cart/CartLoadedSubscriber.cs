using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Cart;

public class CartLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsHandler<CartLoaded>
{
    public async Task Handle(CartLoaded @event, HttpRequest request)
    {
        var productIds = @event.OrderedProducts.Keys.ToList();
        var resultSet = await dbContext.Products
            .Where(s => productIds.Contains(s.ProductId))
            .ToListAsync(request.HttpContext.RequestAborted);

        decimal totalPrice = 0;
        foreach (var product in @event.OrderedProducts)
        {
            var matchingProduct = resultSet.Single(s => s.ProductId == product.Key);
            product.Value.Price = matchingProduct.Price;
            product.Value.Discount = matchingProduct.Discount;

            totalPrice += matchingProduct.EffectivePrice() * (int)product.Value.Quantity;
        }

        var vm = request.GetComposedResponseModel();
        vm.TotalCartPrice = totalPrice;
    }
}
