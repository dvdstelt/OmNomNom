using Catalog.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

// Returns the distinct values for each filter axis the dropdowns
// need to populate. Three small distinct queries; cheap enough to
// compute on every call.
[CompositionHandler]
public class ProductsFacetsComposer(CatalogDbContext dbContext, IHttpContextAccessor http)
{
    [HttpGet("/products/facets")]
    public async Task Handle()
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        var beerStyles = await dbContext.Products
            .Select(p => p.BeerStyle)
            .Where(s => s != null && s != "")
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(ct);

        var breweries = await dbContext.Products
            .Select(p => p.Brewery)
            .Where(b => b != null && b != "")
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync(ct);

        var countries = await dbContext.Products
            .Select(p => p.Country)
            .Where(c => c != null && c != "")
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);

        var vm = request.GetComposedResponseModel();
        vm.BeerStyles = beerStyles;
        vm.Breweries = breweries;
        vm.Countries = countries;
    }
}
