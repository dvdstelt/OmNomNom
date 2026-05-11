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

public class OrderSummaryComposer(FinanceDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/email/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderData = await request.Bind<OrderData>();
        var ct = request.HttpContext.RequestAborted;

        // The email summary fires on OrderShipped, so by this point a
        // delivery option has been chosen; the .Value access matches the
        // pre-SQLite assumption that this is non-null on a placed order.
        var row = await (
            from o in dbContext.Orders.Include(o => o.Items)
            join d in dbContext.DeliveryOptions on o.DeliveryOptionId equals d.DeliveryOptionId
            where o.OrderId == orderData.OrderId
            select new { Order = o, DeliveryOption = d }).SingleAsync(ct);

        var order = row.Order;
        var deliveryOption = row.DeliveryOption;

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
