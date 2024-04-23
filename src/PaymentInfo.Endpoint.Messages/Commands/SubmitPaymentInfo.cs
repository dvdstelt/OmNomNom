namespace PaymentInfo.Endpoint.Messages.Commands;

public class SubmitPaymentInfo
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid CreditCardId { get; set; }
}