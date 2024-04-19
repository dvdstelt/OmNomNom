using Catalog.Data.Models;
using Catalog.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Orders;

public class AddProductToCartHandler(CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpPost("/cart/addproduct/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var productToAdd = await request.Bind<ProductModel>();
        var orderId = productToAdd.orderId;
        var productId = productToAdd.Detail.Id;

        var order = await cacheHelper.GetOrder(orderId);
        var existingOrderItem = order.Products.SingleOrDefault(p => p.Key == productId);
        if (existingOrderItem.Value != null)
        {
            existingOrderItem.Value.Quantity += productToAdd.Detail.Quantity;
            if (existingOrderItem.Value.Quantity <= 0)
                order.Products.TryRemove(existingOrderItem);
        }
        else
        {
            var orderItem = new OrderProduct() { ProductId = productId, Quantity = productToAdd.Detail.Quantity };
            order.Products.TryAdd(productId, orderItem);
        }

        await cacheHelper.StoreOrder(order);

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
    }

    class ProductModel
    {
        [FromRoute] public Guid orderId { get; set; }
        [FromBody] public ProductModelBody Detail { get; set; }
    }

    class ProductModelBody
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
    }
}