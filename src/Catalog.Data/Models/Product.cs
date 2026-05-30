namespace Catalog.Data.Models;

public class Product
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string BeerStyle { get; set; } = null!;
    public string Brewery { get; set; } = null!;
    public string Country { get; set; } = null!;
}