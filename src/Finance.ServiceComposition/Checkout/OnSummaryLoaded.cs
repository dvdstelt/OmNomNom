using Catalog.ServiceComposition.Events;
using Finance.Data.Models;
using Finance.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Checkout;

public class OnSummaryLoaded(OrderSubtotalReader orderReader) : ICompositionEventsHandler<SummaryLoaded>
{
    public async Task Handle(SummaryLoaded @event, HttpRequest request)
    {
        var ct = request.HttpContext.RequestAborted;
        var order = await orderReader.GetOrderWithItemsAsync(@event.OrderId, ct);

        var totalPrice = 0m;

        foreach (var product in @event.Products)
        {
            var matchingProduct = order.Items.SingleOrDefault(s => s.ProductId == product.Key);
            if (matchingProduct is null) continue;
            product.Value.Price = matchingProduct.Price;
            product.Value.Discount = matchingProduct.Discount;
            totalPrice += matchingProduct.EffectivePrice() * matchingProduct.BillableQuantity;
        }

        var vm = request.GetComposedResponseModel();
        DynamicHelper.TrySetTotalPrice(vm, totalPrice);
    }
}
