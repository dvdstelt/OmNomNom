using System.Text.Json;
using Finance.Data;
using Finance.ServiceComposition.Cart;
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
public class OrderSubmitComposer(IWorkflowStore workflow, FinanceDbContext dbContext, IHttpContextAccessor http)
{
    static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [HttpPost("/cart/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;
        // Catalog has a matching OrderSubmitComposer on the same route that
        // also reads the body. ServiceComposer enables buffering, so rewind
        // before each read so whichever composer runs second still sees JSON.
        request.Body.Position = 0;
        var items = await JsonSerializer.DeserializeAsync<List<CartItemForm>>(request.Body, JsonOptions, ct) ?? [];

        // Attach the PriceId locked at add-to-cart. A line missing one (e.g.
        // a quantity edited before this slice existed) falls back to current.
        var captured = await workflow.Read<CartPricesSlice>(orderId, CartPricesWorkflowSlice.Key, ct)
                       ?? CartPricesSlice.Empty;
        var lockedPriceIds = captured.Items.ToDictionary(l => l.ProductId, l => l.PriceId);

        var lines = new List<OrderItemLine>(items.Count);
        foreach (var item in items)
        {
            if (!lockedPriceIds.TryGetValue(item.ProductId, out var priceId))
                priceId = (await dbContext.CurrentPriceAsync(item.ProductId, ct)).PriceId;
            lines.Add(new OrderItemLine(item.ProductId, item.Quantity, priceId));
        }

        await workflow.Write(orderId, OrderItemsWorkflowSlice.Key, new OrderItemsSlice(lines), ct);
    }
}
