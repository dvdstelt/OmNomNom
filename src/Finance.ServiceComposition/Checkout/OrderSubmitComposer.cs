using System.Text.Json;
using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Checkout;

// Body is a raw JSON array of ShoppingCartItem; read it directly from
// request.Body rather than via [FromBody]. See Catalog's matching
// OrderSubmitComposer for the rationale.
[CompositionHandler]
public class OrderSubmitComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [HttpPost("/cart/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;
        var items = await JsonSerializer.DeserializeAsync<List<ShoppingCartItem>>(request.Body, JsonOptions, ct) ?? [];

        var slice = new OrderItemsSlice(items
            .Select(i => new OrderItemLine(i.ProductId, i.Quantity))
            .ToList());
        await workflow.Write(orderId, OrderItemsWorkflowSlice.Key, slice, ct);
    }

    public class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
