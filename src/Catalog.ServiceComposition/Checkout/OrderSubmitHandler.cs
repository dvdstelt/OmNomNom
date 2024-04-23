using Catalog.Endpoint.Messages.Commands;
using Catalog.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Checkout;

public class OrderSubmitHandler(IMessageSession messageSession, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var data = await request.Bind<ShoppingCart>();
        var order = await cacheHelper.GetOrder(data.OrderId);
        
        var message = new SubmitOrderItems();
        message.OrderId = data.OrderId;
        foreach (var item in order.Products)
        {
            message.Items.Add(new OrderItem() { ProductId = item.ProductId, Quantity = item.Quantity });
        }

        await messageSession.Send(message);
    }

    class ShoppingCart
    {
        [FromRoute] public Guid OrderId { get; set; }
    }
}