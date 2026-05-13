namespace Catalog.ServiceComposition.Products;

// Declarative query-string model for GET /products. ServiceComposer's
// model binder fills each property from the corresponding query key
// (case-insensitive). The comma-separated filters arrive as a single
// string so we keep them as strings here and split on access; that
// matches the frontend, which builds `?categories=a,b,c`.
public class ProductsQuery
{
    public string? Categories { get; set; }
    public string? Breweries { get; set; }
    public string? Countries { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 12;

    public IReadOnlyList<string> CategoryList => Split(Categories);
    public IReadOnlyList<string> BreweryList => Split(Breweries);
    public IReadOnlyList<string> CountryList => Split(Countries);

    static List<string> Split(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
}
