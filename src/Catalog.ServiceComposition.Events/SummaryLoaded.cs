namespace Catalog.ServiceComposition.Events;

public class SummaryLoaded
{
    public Guid OrderId { get; set; }
    public IDictionary<Guid, dynamic> Products { get; set; }
}