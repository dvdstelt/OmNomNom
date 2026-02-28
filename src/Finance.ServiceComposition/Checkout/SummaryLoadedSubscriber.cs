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

            var totalPrice = 0m;

            // Use stored order prices only when they have been persisted with real values.
            // The Finance cache carries Price = 0 (set before the SubmitOrderItems message is
            // processed), so falling back to it would produce wrong totals.
            if (order != null && order.Items.Any(i => i.Price > 0))
            {
                foreach (var product in @event.Products)
                {
                    var matchingProduct = order.Items.Single(s => s.ProductId == product.Key);
                    product.Value.Price = matchingProduct.Price;
                    totalPrice += matchingProduct.Quantity * matchingProduct.Price;
                }
            }
            else
            {
                // Order not yet persisted or prices not yet written - look up current prices
                // from the Finance product catalogue, the same source the cart uses.
                var productIds = @event.Products.Keys.ToList();
                var productCollection = dbContext.Database.GetCollection<Finance.Data.Models.Product>();
                var financeProducts = productCollection.Query()
                    .Where(s => productIds.Contains(s.ProductId))
                    .ToList();

                foreach (var product in @event.Products)
                {
                    var financeProduct = financeProducts.Single(s => s.ProductId == product.Key);
                    product.Value.Price = financeProduct.Price;
                    totalPrice += (int)product.Value.Quantity * financeProduct.Price;
                }
            }

            var vm = request.GetComposedResponseModel();
            DynamicHelper.TrySetTotalPrice(vm, totalPrice);
        });
    }
}