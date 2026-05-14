namespace Shipping.Endpoint.Messages.Commands;

public sealed record SubmitShippingAddress
{
    public required Guid OrderId { get; init; }
    public required string FullName { get; init; }
    public required string Street { get; init; }
    public required string ZipCode { get; init; }
    public required string Town { get; init; }
    public required string Country { get; init; }
}
