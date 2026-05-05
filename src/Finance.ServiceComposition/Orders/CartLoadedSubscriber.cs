using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using ITOps.Shared;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Orders;

public class CartLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/cart/{orderId}")]
    [HttpGet("/buy/address/{orderId}")]
    [HttpGet("/buy/shipping/{orderId}")]
    [HttpGet("/buy/payment/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<CartLoaded>((@event, request) =>
        {
            var productIds = @event.OrderedProducts.Keys.ToList();
            var resultSet = dbContext.Where<Product>(s => productIds.Contains(s.ProductId)).ToList();

            decimal totalPrice = 0;
            foreach (var product in @event.OrderedProducts)
            {
                var matchingProduct  = resultSet.Single(s => s.ProductId == product.Key);
                product.Value.Price = matchingProduct.Price;
                product.Value.Discount = matchingProduct.Discount;

                totalPrice += matchingProduct.EffectivePrice() * (int)product.Value.Quantity;
            }

            var vm = request.GetComposedResponseModel();
            vm.TotalCartPrice = totalPrice;

            return Task.CompletedTask;
        });
    }
}