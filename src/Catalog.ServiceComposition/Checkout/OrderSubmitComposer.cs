using System.Text;
using System.Text.Json;
using Catalog.Data.Models;
using Catalog.Endpoint.Messages.Commands;
using Catalog.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using OrderItem = Catalog.Endpoint.Messages.Commands.OrderItem;

namespace Catalog.ServiceComposition.Checkout;

public class OrderSubmitComposer(IMessageSession messageSession, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var data = await request.Bind<ShoppingCart>();

        // Read the cart items straight from the body. The shared cache only
        // tracks Add-to-Cart calls; quantity edits made on the cart page
        // arrive for the first time in this POST.
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        var items = JsonSerializer.Deserialize<List<ShoppingCartItem>>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        // Refresh the shared cache so a later GET /cart/{orderId} reflects
        // what was actually submitted.
        var cacheOrder = new Order { OrderId = data.OrderId };
        foreach (var item in items)
        {
            cacheOrder.Products.Add(new Data.Models.OrderItem
            {
                OrderId = data.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }
        await cacheHelper.StoreOrder(cacheOrder);

        var message = new SubmitOrderItems { OrderId = data.OrderId };
        foreach (var item in items)
        {
            message.Items.Add(new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity });
        }

        await messageSession.Send(message);
    }

    class ShoppingCart
    {
        [FromRoute] public Guid OrderId { get; set; }
    }

    class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}