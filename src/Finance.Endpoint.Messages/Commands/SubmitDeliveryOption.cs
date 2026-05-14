namespace Finance.Endpoint.Messages.Commands;

public sealed record SubmitDeliveryOption
{
    public required Guid OrderId { get; init; }
    public required Guid DeliveryOptionId { get; init; }
}
