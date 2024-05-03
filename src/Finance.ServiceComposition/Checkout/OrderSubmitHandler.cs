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
        
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true );
        var body = await reader.ReadToEndAsync();
        var shoppingCartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        
        var cacheOrder = new Order();
        cacheOrder.OrderId = submitted.OrderId;
        
        var message = new SubmitOrderItems();
        message.OrderId = submitted.OrderId;
        foreach (var item in shoppingCartItems)
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
        public Guid OrderId { get; set; }
        // public string AString { get; set; }
        // public Dictionary<int, ShoppingCartItem> Items { get; set; }
    }

    class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}