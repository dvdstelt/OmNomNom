using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentInfo.Endpoint.Messages.Commands;
using PaymentInfo.ServiceComposition.Workflow;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace PaymentInfo.ServiceComposition.Checkout;

public class PaymentSubmitComposer(IMessageSession messageSession, IWorkflowStore workflow) : ICompositionRequestsHandler
{
    // Same hardcoded id as PaymentWorkflowSlice; placeholder until
    // real authentication is wired in.
    static readonly Guid PlaceholderCustomerId = Guid.Parse("01093176-1308-493a-8f67-da5d278e2375");

    [HttpPost("/buy/payment/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<CreditCard>();
        var ct = request.HttpContext.RequestAborted;

        var slice = new PaymentSlice(submitted.Body.CreditCardId);
        await workflow.Write(submitted.OrderId, PaymentWorkflowSlice.Key, slice, ct);

        var message = new SubmitPaymentInfo
        {
            OrderId = submitted.OrderId,
            CreditCardId = submitted.Body.CreditCardId,
            CustomerId = PlaceholderCustomerId
        };
        await messageSession.Send(message);
    }

    class CreditCard
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public PaymentBody Body { get; set; } = null!;
    }

    class PaymentBody
    {
        public Guid CreditCardId { get; set; }
    }
}
