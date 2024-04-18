namespace Catalog.ServiceComposition.Events;

public class ProductLoaded
{
    public Guid ProductId { get; set; }
    public dynamic Product { get; set; }
}