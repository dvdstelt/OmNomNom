namespace PaymentInfo.Endpoint.Messages.Commands;

public sealed record SubmitPaymentInfo
{
    public required Guid OrderId { get; init; }
    public required Guid CustomerId { get; init; }
    public required Guid CreditCardId { get; init; }
}
