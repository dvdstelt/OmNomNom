using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PaymentInfo.Data;
using PaymentInfo.ServiceComposition.Helpers;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition.Checkout;

public class SummaryHandler(PaymentInfoDbContext dbContext, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var order = await dbContext.Orders
            .FirstOrDefaultAsync(s => s.OrderId == orderId, ct);
        if (order == null)
        {
            order = await cacheHelper.GetOrder(orderId);
        }

        var creditCard = await dbContext.CreditCards
            .FirstOrDefaultAsync(s => s.CreditCardId == order.CreditCardId, ct);

        var vm = request.GetComposedResponseModel();
        vm.CreditCardLastDigits = creditCard?.LastDigits;
        vm.CreditCardType = creditCard?.CardType;
    }
}
