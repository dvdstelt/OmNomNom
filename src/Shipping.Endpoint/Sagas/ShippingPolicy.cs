using Catalog.Endpoint.Messages.Events;
using Finance.Endpoint.Messages.Events;
using Microsoft.Extensions.Logging;
using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Messages.Events;
using Shipping.Endpoint.Messages.Messages;

namespace Shipping.Endpoint.Sagas;

public class ShippingPolicy(ILogger<ShippingPolicy> log) : Saga<ShippingPolicyData>,
    IAmStartedByMessages<OrderAccepted>,
    IAmStartedByMessages<PaymentSucceeded>,
    IHandleMessages<ShipOrderReply>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingPolicyData> mapper)
    {
        mapper.MapSaga(s => s.OrderId)
            .ToMessage<OrderAccepted>(m => m.OrderId)
            .ToMessage<PaymentSucceeded>(m => m.OrderId);
    }

    public async Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        log.LogInformation("{OrderId} - OrderAccepted received", Data.OrderId);

        Data.OrderAccepted = true;
        await CanWeContinue(context);
    }

    public async Task Handle(PaymentSucceeded message, IMessageHandlerContext context)
    {
        log.LogInformation("{OrderId} - PaymentSucceeded", Data.OrderId);

        Data.PaymentSucceeded = true;
        await CanWeContinue(context);
    }

    async Task CanWeContinue(IMessageHandlerContext context)
    {
        if (Data.OrderAccepted && Data.PaymentSucceeded)
        {
            log.LogInformation("{OrderId} - Continue with processing", Data.OrderId);

            var requestMessage = new ShipOrderRequest { OrderId = Data.OrderId };
            await context.Send(requestMessage);
        }
    }

    public async Task Handle(ShipOrderReply message, IMessageHandlerContext context)
    {
        log.LogInformation("{OrderId} - ShipOrderReply received", Data.OrderId);

        var @event = new OrderShipped { OrderId = Data.OrderId };

        await context.Publish(@event);

        log.LogInformation("{OrderId} - OrderShipped published", Data.OrderId);

        // Can we actually mark this as complete? Or would there be more process?
        MarkAsComplete();
    }
}

public class ShippingPolicyData : ContainSagaData
{
    public Guid OrderId { get; set; }
    public bool PaymentSucceeded { get; set; } = false;
    public bool OrderAccepted { get; set; } = false;
}
