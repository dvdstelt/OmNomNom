using OmNomNom.Website.Helpers;
using Shipping.Endpoint.Messages.Events;

namespace OmNomNom.Website.Handlers;

public class OrderShippedHandler(OrderEmailSender emailSender) : IHandleMessages<OrderShipped>
{
    public async Task Handle(OrderShipped message, IMessageHandlerContext context)
    {
        var ct = context.CancellationToken;
        var content = await emailSender.GetOrderSummaryAsync(message.OrderId, ct);

        // Pick the template based on whether anything in this order
        // came back marked Fulfilled=false. The summary always carries
        // both fulfilled and unfulfilled lines; an order with mixed
        // outcomes gets the partial template (which lists what was
        // missed and offers the replace-items link).
        var anyUnfulfilled = content["products"]?
            .Any(p => p["fulfilled"]?.ToObject<bool>() == false) ?? false;

        if (anyUnfulfilled)
        {
            await emailSender.SendAsync(
                viewPath: "/Views/Emails/PartiallyFulfilledEmailContent.cshtml",
                subject: "Your OmNomNom order shipped (some items unavailable)",
                content: content,
                ct: ct);
        }
        else
        {
            await emailSender.SendAsync(
                viewPath: "/Views/Emails/EmailContent.cshtml",
                subject: "Thanks for ordering with OmNomNom",
                content: content,
                ct: ct);
        }
    }
}
