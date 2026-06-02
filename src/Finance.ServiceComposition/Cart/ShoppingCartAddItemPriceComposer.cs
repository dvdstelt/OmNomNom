using System.Text.Json;
using Finance.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Cart;

// Co-runs with Catalog's ShoppingCartAddItemComposer on the same route.
// Catalog records ProductId + Quantity in its CartSlice; Finance records
// the PriceId the customer saw in its own CartPricesSlice, so the price
// is locked the moment the item enters the cart. Reads the buffered body
// directly (ServiceComposer enables buffering) and rewinds first so the
// other composer still sees JSON.
[CompositionHandler]
public class ShoppingCartAddItemPriceComposer(
    IWorkflowStore workflow,
    FinanceDbContext dbContext,
    IHttpContextAccessor http)
{
    static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [HttpPost("/cart/addproduct/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        request.Body.Position = 0;
        var form = await JsonSerializer.DeserializeAsync<AddProductPriceForm>(request.Body, JsonOptions, ct);
        if (form is null || form.Id == Guid.Empty)
            return;

        var slice = await workflow.Read<CartPricesSlice>(orderId, CartPricesWorkflowSlice.Key, ct)
                    ?? CartPricesSlice.Empty;

        var priceId = await ResolvePriceId(form, slice, ct);

        var lines = slice.Items.Where(l => l.ProductId != form.Id).ToList();
        lines.Add(new CartPriceLine(form.Id, priceId));
        await workflow.Write(orderId, CartPricesWorkflowSlice.Key, new CartPricesSlice(lines), ct);
    }

    // The on-screen PriceId forwarded by the UI is authoritative. When it
    // is absent (e.g. a quantity tweak from the cart page posts only
    // id+quantity), keep the price already locked for this product;
    // only resolve the current price when nothing has been captured yet.
    async Task<Guid> ResolvePriceId(AddProductPriceForm form, CartPricesSlice slice, CancellationToken ct)
    {
        if (form.PriceId != Guid.Empty)
            return form.PriceId;

        var existing = slice.Items.FirstOrDefault(l => l.ProductId == form.Id);
        if (existing is not null)
            return existing.PriceId;

        var current = await dbContext.CurrentPriceAsync(form.Id, ct);
        return current.PriceId;
    }

    sealed class AddProductPriceForm
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public Guid PriceId { get; set; }
    }
}
