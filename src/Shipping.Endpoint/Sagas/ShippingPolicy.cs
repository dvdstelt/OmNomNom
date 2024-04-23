using Catalog.Endpoint.Messages.Events;
using NServiceBus.Logging;
using PaymentInfo.Endpoint.Messages.Events;
using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Messages.Events;
using Shipping.Endpoint.Messages.Messages;

namespace Shipping.Endpoint.Sagas;

public class ShippingPolicy : Saga<ShippingPolicyData>,
    IAmStartedByMessages<OrderAccepted>,
    IAmStartedByMessages<PaymentSucceeded>,
    IHandleMessages<ShipOrderReply>
{
    static ILog log = LogManager.GetLogger<ShippingPolicy>();
    
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingPolicyData> mapper)
    {
        mapper.MapSaga(s => s.OrderId)
            .ToMessage<OrderAccepted>(m => m.OrderId)
            .ToMessage<PaymentSucceeded>(m => m.OrderId);
    }

    public async Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        log.InfoFormat("{OrderId} - OrderAccepted received", Data.OrderId);

        Data.OrderAccepted = true;
        await CanWeContinue(context);
    }

    public async Task Handle(PaymentSucceeded message, IMessageHandlerContext context)
    {
        log.InfoFormat("{OrderId} - PaymentSucceeded", Data.OrderId);

        Data.PaymentSucceeded = true;
        await CanWeContinue(context);
    }

    async Task CanWeContinue(IMessageHandlerContext context)
    {
        if (Data.OrderAccepted && Data.PaymentSucceeded)
        {
            log.InfoFormat("{OrderId} - Continue with processing", Data.OrderId);

            var requestMessge = new ShipOrderRequest();
            requestMessge.OrderId = Data.OrderId;
            await context.Send(requestMessge);
        }
    }

    public async Task Handle(ShipOrderReply message, IMessageHandlerContext context)
    {
        log.InfoFormat("{OrderId} - ShipOrderReply received", Data.OrderId);
        
        var @event = new OrderShipped();
        @event.OrderId = Data.OrderId;

        await context.Publish(@event);

        log.InfoFormat("{OrderId} - OrderShipped published", Data.OrderId);
        
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