using Catalog.ServiceComposition.Events;
using Finance.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Email;

// Per-order pass over the email products dictionary: any line whose
// Finance OrderItem is marked Fulfilled=false (set by
// Finance.Endpoint.Handlers.OrderPlacedHandler /
// OrderCancelledHandler) gets Fulfilled=false on its email view-model,
// so the template can render it under "couldn't be fulfilled" with
// the corresponding messaging.
class OnOrderEmailProductsLoaded(FinanceDbContext dbContext) : ICompositionEventsHandler<OrderEmailProductsLoaded>
{
    public async Task Handle(OrderEmailProductsLoaded @event, HttpRequest request)
    {
        var unfulfilledIds = await dbContext.OrderItems
            .Where(i => i.OrderId == @event.OrderId && !i.Fulfilled)
            .Select(i => i.ProductId)
            .ToListAsync(request.HttpContext.RequestAborted);

        foreach (var productId in unfulfilledIds)
        {
            if (@event.Products.TryGetValue(productId, out var product))
            {
                product.Fulfilled = false;
            }
        }
    }
}
