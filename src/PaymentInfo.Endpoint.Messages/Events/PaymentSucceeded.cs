namespace PaymentInfo.Endpoint.Messages.Events;

public class PaymentSucceeded
{
    public Guid OrderId { get; set; }
}