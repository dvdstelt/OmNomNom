using System.Text;
using System.Text.Json;
using Finance.Endpoint.Messages.Commands;
using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;
using OrderItem = Finance.Endpoint.Messages.Commands.OrderItem;

namespace Finance.ServiceComposition.Checkout;

public class OrderSubmitComposer(IMessageSession messageSession, IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<ShoppingCart>();
        var ct = request.HttpContext.RequestAborted;

        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync(ct);
        var shoppingCartItems = JsonSerializer.Deserialize<List<ShoppingCartItem>>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        var slice = new OrderItemsSlice(shoppingCartItems
            .Select(i => new OrderItemLine(i.ProductId, i.Quantity))
            .ToList());
        await workflow.Write(submitted.OrderId, OrderItemsWorkflowSlice.Key, slice, ct);

        var message = new SubmitOrderItems { OrderId = submitted.OrderId };
        foreach (var item in shoppingCartItems)
        {
            message.Items.Add(new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity });
        }
        await messageSession.Send(message);
    }

    class ShoppingCart
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public ShoppingCartModel Model { get; set; } = null!;
    }

    class ShoppingCartModel
    {
        public Guid OrderId { get; set; }
    }

    class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
