using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PaymentInfo.Data;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition.Checkout;

public class PaymentHandler(PaymentInfoDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/payment/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var order = await dbContext.Orders
            .FirstOrDefaultAsync(s => s.OrderId == orderId, request.HttpContext.RequestAborted);

        vm.CreditCardId = order?.CreditCardId;
    }
}
