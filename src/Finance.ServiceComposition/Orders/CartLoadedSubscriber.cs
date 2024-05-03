using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Orders;

public class CartLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/cart/{orderId}")]
    [HttpGet("/buy/shipping/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<CartLoaded>((@event, request) =>
        {
            var productsCollection = dbContext.Database.GetCollection<Product>();
            var productIds = @event.OrderedProducts.Keys.ToList();
            var resultSet = productsCollection.Query().Where(s => productIds.Contains(s.ProductId)).ToList();

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