using System.Text;
using System.Text.Json;
using Catalog.Endpoint.Messages.Commands;
using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;
using OrderItem = Catalog.Endpoint.Messages.Commands.OrderItem;

namespace Catalog.ServiceComposition.Checkout;

// Cart-page submit: the user clicked "Proceed to checkout" on /cart.
// Body carries the final line items (quantity edits and removes happen
// inline on the cart page and only land here). We rewrite the cart
// slice with the body's items and continue to dispatch
// SubmitOrderItems directly via IMessageSession - that send becomes
// part of the WorkflowSubmitter's atomic fan-out once all four
// service boundaries have migrated to slices.
public class OrderSubmitComposer(IMessageSession messageSession, IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var data = await request.Bind<ShoppingCart>();
        var ct = request.HttpContext.RequestAborted;

        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync(ct);
        var items = JsonSerializer.Deserialize<List<ShoppingCartItem>>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        var slice = new CartSlice(items
            .Select(i => new CartLine(i.ProductId, i.Quantity))
            .ToList());
        await workflow.Write(data.OrderId, CartWorkflowSlice.Key, slice, ct);

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
