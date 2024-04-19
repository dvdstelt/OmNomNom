using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Orders;

public class CartLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsSubscriber
{
    readonly FinanceDbContext dbContext = dbContext;

    [HttpGet("/cart/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<CartLoaded>((@event, request) =>
        {
            var productsCollection = dbContext.Database.GetCollection<Product>();
            // TODO: Figure out if `Contains` is possible with LiteDb
            var resultSet = productsCollection.Query().ToList();

            decimal totalPrice = 0;

            foreach (var product in @event.OrderedProducts)
            {
                var matchingProduct  = resultSet.Single(s => s.ProductId == product.Key);
                product.Value.Price = matchingProduct.Price;
                product.Value.Discount = matchingProduct.Discount;

                var itemPrice = matchingProduct.Price * (int)product.Value.Quantity;
                var discount = itemPrice / 100 * matchingProduct.Discount;
                totalPrice += itemPrice - discount;
            }

            var vm = request.GetComposedResponseModel();
            vm.TotalCartPrice = totalPrice;

            return Task.CompletedTask;
        });
    }
}