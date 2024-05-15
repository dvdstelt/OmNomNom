using Catalog.Data.Models;
using Catalog.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.ShoppingCart;

public class ShoppingCartAddItemHandler(CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpPost("/cart/addproduct/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var productToAdd = await request.Bind<ProductModel>();
        var orderId = productToAdd.orderId;
        var productId = productToAdd.Detail.Id;

        if (orderId == Guid.Empty)
            orderId = Guid.NewGuid();

        var order = await cacheHelper.GetOrder(orderId);
        UpsertProduct(order, productId, productToAdd);
        await cacheHelper.StoreOrder(order);

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
    }

    private static void UpsertProduct(Order order, Guid productId, ProductModel productToAdd)
    {
        var existingOrderItem = order.Products.SingleOrDefault(p => p.ProductId == productId);
        if (existingOrderItem != null)
        {
            existingOrderItem.Quantity += productToAdd.Detail.Quantity;
            if (existingOrderItem.Quantity <= 0)
                order.Products.Remove(existingOrderItem);
        }
        else
        {
            var orderItem = new OrderItem() { ProductId = productId, Quantity = productToAdd.Detail.Quantity };
            order.Products.Add(orderItem);
        }
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