namespace Catalog.ServiceComposition.Events;

public class ProductsLoaded
{
    public IDictionary<Guid, dynamic> Products { get; set; }
}