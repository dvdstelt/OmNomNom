using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

// Server-side product search. Catalog owns the result set and applies
// filters on Catalog-owned fields (category/brewery/country).
// Sorting on signals Catalog doesn't own is delegated by raising
// ProductCandidatesAvailable: any subscriber that owns a sort signal
// (e.g. Marketing) reads its own request parameters and writes the
// ordered IDs back. Catalog has no opinion on the sort vocabulary and
// never reads ?sort= itself.
//
// Query parameters bind as scalar [FromQuery] parameters. A complex
// ProductsQuery type was tried but ServiceComposer's source generator
// emits [BindFromQuery<T>("query")] for it, which makes MVC's
// ComplexObjectModelBinder require a "query." prefix on every key -
// the frontend sends bare ?page=, and Marketing reads ?sort= straight
// off request.Query, so any prefix would break one or both.
//
// The composed response carries:
//   Products    list of dictionary values for the page (in order)
//   Page, PageSize, TotalCount, TotalPages
[CompositionHandler]
public class ProductsComposer(CatalogDbContext dbContext, IHttpContextAccessor http)
{
    const int MaxPageSize = 50;

    [HttpGet("/products")]
    public async Task Handle(
        [FromQuery] string? categories,
        [FromQuery] string? breweries,
        [FromQuery] string? countries,
        [FromQuery] int page,
        [FromQuery] int size)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        // ServiceComposer ignores C# parameter default values and its
        // source generator rejects nullable-int parameters (the '?'
        // breaks the generator's hintName), so treat the bound 0 - what
        // model binding produces when ?page=/?size= is missing - as
        // "fall back to the canonical default" before clamping.
        var pageNumber = page > 0 ? page : 1;
        var pageSize = Math.Clamp(size > 0 ? size : 12, 1, MaxPageSize);

        var categoryList = Split(categories);
        var breweryList = Split(breweries);
        var countryList = Split(countries);

        // 1. Filter Catalog-owned attributes. Each filter only applies
        //    when the caller supplied at least one value.
        var productsQuery = dbContext.Products.AsQueryable();
        if (categoryList.Count > 0) productsQuery = productsQuery.Where(p => categoryList.Contains(p.Category));
        if (breweryList.Count > 0) productsQuery = productsQuery.Where(p => breweryList.Contains(p.Brewery));
        if (countryList.Count > 0) productsQuery = productsQuery.Where(p => countryList.Contains(p.Country));

        // 2. Materialise the filtered candidate set as IDs only. We
        //    re-query via productsQuery for the fallback page slice
        //    if no subscriber claims the sort.
        var candidateIds = await productsQuery.Select(p => p.ProductId).ToListAsync(ct);
        var totalCount = candidateIds.Count;

        // 3. Ask other components to claim ownership of the sort. Each
        //    subscriber inspects whatever request parameters it cares
        //    about and fills OrderedIds. If none does (no sort param,
        //    unknown sort, or no subscriber deployed) Catalog falls
        //    back to its own default sort by Name.
        var context = request.GetCompositionContext();
        var sortRequest = new ProductCandidatesAvailable
        {
            CandidateIds = candidateIds,
            Page = pageNumber,
            Size = pageSize
        };
        await context.RaiseEvent(sortRequest);

        var pageIds = sortRequest.OrderedIds?.ToList()
            ?? await productsQuery
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.ProductId)
                .ToListAsync(ct);

        // 4. Load the Product rows for the page plus the live in-stock
        //    figure (sum of the InventoryDelta ledger) and drop the
        //    sold-out ones. Worst case the page shows fewer than `size`
        //    items rather than a "Less than 1 left" tile.
        var pageRows = await dbContext.Products
            .Where(p => pageIds.Contains(p.ProductId))
            .Select(p => new
            {
                Product = p,
                InStock = dbContext.InventoryDeltas
                    .Where(d => d.ProductId == p.ProductId)
                    .Sum(d => (int?)d.Delta) ?? 0
            })
            .Where(x => x.InStock > 0)
            .ToListAsync(ct);

        var lookup = pageRows.ToDictionary(x => x.Product.ProductId);

        var productsModel = new Dictionary<Guid, dynamic>();
        foreach (var id in pageIds)
        {
            if (!lookup.TryGetValue(id, out var row)) continue;
            productsModel[id] = Mapper.MapToViewModel(row.Product, row.InStock);
        }

        // 5. Compose: Marketing/Finance subscribers attach their
        //    slices to the page items (not the whole catalog).
        await context.RaiseEvent(new ProductsLoaded
        {
            Products = productsModel
        });

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var responseModel = request.GetComposedResponseModel();

        // Drive the response array from the explicit ordered key list
        // rather than productsModel.Values — Dictionary enumeration
        // order is documented as unspecified and must not be the
        // source of truth for a user-visible sort.
        responseModel.Products = pageIds
            .Where(productsModel.ContainsKey)
            .Select(id => productsModel[id])
            .ToList();
        responseModel.Page = pageNumber;
        responseModel.PageSize = pageSize;
        responseModel.TotalCount = totalCount;
        responseModel.TotalPages = totalPages;
    }

    static List<string> Split(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
}
