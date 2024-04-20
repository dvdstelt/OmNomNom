using Finance.Endpoint.Messages.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Checkout;

public class OrderSubmitHandler(IMessageSession messageSession) : ICompositionRequestsHandler
{
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
        public Guid OrderId { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new();
    }

    class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public Guid PriceId { get; set; }
        public int Quantity { get; set; }
    }

}