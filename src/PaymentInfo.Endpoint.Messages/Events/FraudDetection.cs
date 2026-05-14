namespace PaymentInfo.Endpoint.Messages.Events;

public sealed record FraudDetection
{
    public required Guid CustomerId { get; init; }
    public required Guid OrderId { get; init; }
    public required Guid CreditCardId { get; init; }
    public required string FraudMessage { get; init; }
}
