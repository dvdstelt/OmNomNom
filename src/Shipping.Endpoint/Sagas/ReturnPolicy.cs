using Shipping.Endpoint.Messages.Events;

namespace Shipping.Endpoint.Sagas;

public class ReturnPolicy : Saga<ReturnPolicy.ReturnPolicyData>,
    IHandleMessages<OrderShipped>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ReturnPolicyData> mapper)
    {
        mapper.MapSaga(s => s.OrderId)
            .ToMessage<OrderShipped>(m => m.OrderId);
    }

    public Task Handle(OrderShipped message, IMessageHandlerContext context)
    {
        // We do nothing
        // OrderId from message was copied because of mapping
        // We now wait if we get messages that a customer wants to return items.
        return Task.CompletedTask;
    }
    
    public class ReturnPolicyData : ContainSagaData
    {
        public Guid OrderId { get; set; }
    }
}

