namespace Shipping.Endpoint.Messages.Commands;

public class SubmitShippingAddress
{
    public Guid OrderId { get; set; }
    public string FullName { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string Town { get; set; }
    public string Country { get; set; }
}