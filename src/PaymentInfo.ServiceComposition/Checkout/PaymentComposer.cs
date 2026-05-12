using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentInfo.Data;
using PaymentInfo.ServiceComposition.Workflow;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace PaymentInfo.ServiceComposition.Checkout;

[CompositionHandler]
public class PaymentComposer(PaymentInfoDbContext dbContext, IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpGet("/buy/payment/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var vm = request.GetComposedResponseModel();
        var ct = request.HttpContext.RequestAborted;

        // The slice is the user's just-selected card (synchronously
        // written by PaymentSubmitHandler). Fall back to the DB for
        // orders submitted previously.
        var slice = await workflow.Read<PaymentSlice>(orderId, PaymentWorkflowSlice.Key, ct);
        if (slice is not null)
        {
            vm.CreditCardId = slice.CreditCardId;
            return;
        }

        var order = await dbContext.Orders
            .FirstOrDefaultAsync(s => s.OrderId == orderId, ct);

        vm.CreditCardId = order?.CreditCardId;
    }
}
