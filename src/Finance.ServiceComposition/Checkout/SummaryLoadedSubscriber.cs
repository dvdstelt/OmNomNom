using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using Finance.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Checkout;

public class SummaryLoadedSubscriber(FinanceDbContext dbContext, CacheHelper cacheHelper) : ICompositionEventsSubscriber
{
    [HttpGet("/buy/summary/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<SummaryLoaded>(async (@event, request) =>
        {
            var orderId = @event.OrderId;
            var ct = request.HttpContext.RequestAborted;

            var order = await dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(s => s.OrderId == orderId, ct);

            if (order == null)
            {
                order = await cacheHelper.GetOrder(orderId);
            }

            var totalPrice = 0m;

            foreach (var product in @event.Products)
            {
                var matchingProduct = order.Items.Single(s => s.ProductId == product.Key);
                product.Value.Price = matchingProduct.Price;
                product.Value.Discount = matchingProduct.Discount;
                totalPrice += matchingProduct.EffectivePrice() * matchingProduct.Quantity;
            }

            var vm = request.GetComposedResponseModel();
            DynamicHelper.TrySetTotalPrice(vm, totalPrice);
        });
    }
}
