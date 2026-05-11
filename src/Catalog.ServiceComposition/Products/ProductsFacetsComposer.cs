using Catalog.Data;
using Catalog.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

// Returns the distinct values for each filter axis the dropdowns
// need to populate. Restricted to in-stock products to match the
// default ProductsComposer filter - otherwise the UI would offer
// breweries/countries whose only beers are sold out, leading to a
// "No beers match your filter" page after the user picks one.
//
// Three small distinct queries; cheap enough to compute on every call.
public class ProductsFacetsComposer(CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/products/facets")]
    public async Task Handle(HttpRequest request)
    {
        var ct = request.HttpContext.RequestAborted;

        IQueryable<Product> inStockProducts =
            from p in dbContext.Products
            join s in dbContext.InventorySnapshots on p.ProductId equals s.ProductId
            where s.EstimatedInStock > 0
            select p;

        var categories = await inStockProducts
            .Select(p => p.Category)
            .Where(c => c != null && c != "")
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);

        var breweries = await inStockProducts
            .Select(p => p.Brewery)
            .Where(b => b != null && b != "")
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync(ct);

        var countries = await inStockProducts
            .Select(p => p.Country)
            .Where(c => c != null && c != "")
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);

        var vm = request.GetComposedResponseModel();
        vm.Categories = categories;
        vm.Breweries = breweries;
        vm.Countries = countries;
    }
}
