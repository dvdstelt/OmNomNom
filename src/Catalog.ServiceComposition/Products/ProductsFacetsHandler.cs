using Catalog.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

// Returns the full set of distinct values for each filter axis.
// The frontend used to derive these from the in-memory list; with
// server-side pagination the client never sees the whole catalog,
// so the multi-select dropdowns need a separate source. Three small
// distinct queries; cheap enough to compute on every call.
public class ProductsFacetsHandler(CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/products/facets")]
    public async Task Handle(HttpRequest request)
    {
        var ct = request.HttpContext.RequestAborted;

        var categories = await dbContext.Products
            .Select(p => p.Category)
            .Where(c => c != null && c != "")
            .Distinct()
            .OrderBy(c => c)
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
        vm.Categories = categories;
        vm.Breweries = breweries;
        vm.Countries = countries;
    }
}
