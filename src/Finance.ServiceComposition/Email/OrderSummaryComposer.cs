using System.Dynamic;
using Finance.Data;
using Finance.Data.Models;
using Finance.ServiceComposition.Events;
using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.Email;

[CompositionHandler]
public class OrderSummaryComposer(FinanceDbContext dbContext, IHttpContextAccessor http)
{
    [HttpGet("/email/summary/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        // The email summary fires on OrderShipped, so by this point a
        // delivery option has been chosen; the .Value access matches the
        // pre-SQLite assumption that this is non-null on a placed order.
        var row = await (
            from o in dbContext.Orders.Include(o => o.Items).Include(o => o.BillingAddress)
            join d in dbContext.DeliveryOptions on o.DeliveryOptionId equals d.DeliveryOptionId
            where o.OrderId == orderId
            select new { Order = o, DeliveryOption = d }).SingleAsync(ct);

        var order = row.Order;
        var deliveryOption = row.DeliveryOption;
        var billingAddress = order.BillingAddress!;

        dynamic deliveryOptionModel = new ExpandoObject();
        deliveryOptionModel.Price = deliveryOption.Price;

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new DeliveryOptionLoaded()
        {
            DeliveryOptionId = deliveryOption.DeliveryOptionId,
            DeliveryOption = deliveryOptionModel
        });

        var vm = request.GetComposedResponseModel();
        vm.BillingAddress = new BillingAddressData(
            billingAddress.FullName,
            billingAddress.Street,
            billingAddress.ZipCode,
            billingAddress.Town,
            billingAddress.Country);
        vm.DeliveryOption = deliveryOptionModel;
        vm.TotalPrice = order.Items.Sum(s => s.EffectivePrice() * s.Quantity) + deliveryOption.Price;
    }
}
