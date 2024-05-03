using System.Text;
using System.Text.Json;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;
using Finance.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ServiceComposer.AspNetCore;
using OrderItem = Finance.Endpoint.Messages.Commands.OrderItem;

namespace Finance.ServiceComposition.Checkout;

public class OrderSubmitHandler(IMessageSession messageSession, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<ShoppingCart>();

        var cacheOrder = new Order
        {
            OrderId = submitted.OrderId,
            LocationId = submitted.Model.LocationId
        };

        var message = new SubmitOrderItems
        {
            OrderId = submitted.OrderId,
            LocationId = submitted.Model.LocationId
        };
        foreach (var item in submitted.Model.Items)
        {
            message.Items.Add(new OrderItem() { ProductId = item.ProductId, Quantity = item.Quantity });
            cacheOrder.Items.Add(new Data.Models.OrderItem()
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        await cacheHelper.StoreOrder(cacheOrder);
        await messageSession.Send(message);
    }

    class ShoppingCart
    {
        [FromRoute]public Guid OrderId { get; set; }
        [FromBody]public ShoppingCartModel Model { get; set; }
    }

    class ShoppingCartModel
    {
        public Guid LocationId { get; set; }
        public ShoppingCartItem[] Items { get; set; }
    }

    class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}