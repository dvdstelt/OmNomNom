using Catalog.Endpoint.Messages.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Checkout;

public class OrderSubmitHandler(IMessageSession messageSession) : ICompositionRequestsHandler
{
    readonly IMessageSession messageSession = messageSession;

    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<ShoppingCart>();

        var message = new SubmitOrderItems();
        message.OrderId = submitted.OrderId;
        foreach (var item in submitted.Items)
        {
            message.Items.Add(new OrderItem() { ProductId = item.ProductId, Quantity = item.Quantity });
        }

        await messageSession.Send(message);
    }

    class ShoppingCart
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public List<ShoppingCartItem> Items { get; set; }
    }

    class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}