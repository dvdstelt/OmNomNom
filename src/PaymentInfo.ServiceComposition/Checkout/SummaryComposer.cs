using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PaymentInfo.Data;
using PaymentInfo.ServiceComposition.Workflow;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace PaymentInfo.ServiceComposition.Checkout;

public class SummaryComposer(PaymentInfoDbContext dbContext, IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var row = await (
            from o in dbContext.Orders
            where o.OrderId == orderId
            from c in dbContext.CreditCards.Where(c => c.CreditCardId == o.CreditCardId).DefaultIfEmpty()
            select new { Order = o, CreditCard = c })
            .FirstOrDefaultAsync(ct);

        Data.Models.CreditCard? creditCard;
        if (row != null)
        {
            creditCard = row.CreditCard;
        }
        else
        {
            // Order not yet persisted; fall back to the in-flight slice.
            var slice = await workflow.Read<PaymentSlice>(orderId, PaymentWorkflowSlice.Key, ct);
            creditCard = slice is null
                ? null
                : await dbContext.CreditCards
                    .FirstOrDefaultAsync(s => s.CreditCardId == slice.CreditCardId, ct);
        }

        var vm = request.GetComposedResponseModel();
        vm.CreditCardLastDigits = creditCard?.LastDigits;
        vm.CreditCardType = creditCard?.CardType;
    }
}
