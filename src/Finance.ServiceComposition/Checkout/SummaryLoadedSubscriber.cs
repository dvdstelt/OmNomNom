using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using Finance.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Mvc;
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

            var orderCollection = dbContext.Database.GetCollection<Order>();
            var order = orderCollection.Query().Where(s => s.OrderId == orderId).SingleOrDefault();

            if (order == null)
            {
                order = await cacheHelper.GetOrder(orderId);
            }
            
            var totalPrice = 0m;

            foreach (var product in @event.Products)
            {
                var matchingProduct = order.Items.Single(s => s.ProductId == product.Key);
                product.Value.Price = matchingProduct.Price;
                totalPrice += matchingProduct.Quantity * matchingProduct.Price;
            }

            var vm = request.GetComposedResponseModel();
            vm.TotalPrice += totalPrice;
        });
    }
}