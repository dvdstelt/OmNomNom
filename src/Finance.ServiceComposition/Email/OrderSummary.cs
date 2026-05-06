using System.Dynamic;
using Finance.Data;
using Finance.Data.Models;
using Finance.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.Email;

public class OrderSummary(FinanceDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/email/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderData = await request.Bind<OrderData>();
        var ct = request.HttpContext.RequestAborted;

        var order = await dbContext.Orders
            .Include(o => o.Items)
            .SingleAsync(s => s.OrderId == orderData.OrderId, ct);

        // The email summary fires on OrderShipped, so by this point a
        // delivery option has been chosen; the .Value access matches the
        // pre-SQLite assumption that this is non-null on a placed order.
        var deliveryOption = await dbContext.DeliveryOptions
            .SingleAsync(s => s.DeliveryOptionId == order.DeliveryOptionId!.Value, ct);

        dynamic deliveryOptionModel = new ExpandoObject();
        deliveryOptionModel.Price = deliveryOption.Price;

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new DeliveryOptionLoaded()
        {
            DeliveryOptionId = deliveryOption.DeliveryOptionId,
            DeliveryOption = deliveryOptionModel
        });

        var vm = request.GetComposedResponseModel();
        vm.BillingAddress = order.BillingAddress;
        vm.DeliveryOption = deliveryOptionModel;
        vm.TotalPrice = order.Items.Sum(s => s.EffectivePrice() * s.Quantity) + deliveryOption.Price;
    }

    class OrderData
    {
        [FromRoute] public Guid OrderId { get; set; }
    }
}
