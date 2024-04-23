namespace PaymentInfo.Endpoint.Messages.Events;

public class FraudDetection
{
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public Guid CreditCardId { get; set; }
    public string FraudMessage { get; set; }
}