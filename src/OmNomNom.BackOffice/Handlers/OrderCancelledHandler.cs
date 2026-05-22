using Catalog.Endpoint.Messages.Events;
using OmNomNom.Website.Helpers;

namespace OmNomNom.Website.Handlers;

// Fires when Catalog could not fulfil any item in the order. Shipping
// never happens, so OrderShipped never fires - we send a dedicated
// cancellation email instead.
[Handler]
public class OrderCancelledHandler(OrderEmailSender emailSender) : IHandleMessages<OrderCancelled>
{
    public async Task Handle(OrderCancelled message, IMessageHandlerContext context)
    {
        var ct = context.CancellationToken;
        var content = await emailSender.GetOrderSummaryAsync(message.OrderId, ct);

        await emailSender.SendAsync(
            viewPath: "/Views/Emails/OrderCancelledEmailContent.cshtml",
            subject: "Sorry - your OmNomNom order was cancelled",
            content: content,
            ct: ct);
    }
}
