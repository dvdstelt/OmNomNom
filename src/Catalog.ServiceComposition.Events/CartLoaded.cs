namespace Catalog.ServiceComposition.Events;

public class CartLoaded
{
    public Guid OrderId { get; set; }
    public IDictionary<Guid, dynamic> OrderedProducts { get; set; }
}