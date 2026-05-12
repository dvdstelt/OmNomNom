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
// Query parameters live on ProductsQuery; the body of this handler
// stays focused on the search/compose pipeline.
//
// The composed response carries:
//   Products    list of dictionary values for the page (in order)
//   Page, PageSize, TotalCount, TotalPages
[CompositionHandler]
public class ProductsComposer(CatalogDbContext dbContext, IHttpContextAccessor http)
{
    const int MaxPageSize = 50;

    [HttpGet("/products")]
    public async Task Handle([FromQuery] ProductsQuery query)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        var page = Math.Max(1, query.Page);
        var size = Math.Clamp(query.Size, 1, MaxPageSize);

        // 1. Filter Catalog-owned attributes. Each filter only applies
        //    when the caller supplied at least one value.
        var productsQuery = dbContext.Products.AsQueryable();
        if (query.CategoryList.Count > 0) productsQuery = productsQuery.Where(p => query.CategoryList.Contains(p.Category));
        if (query.BreweryList.Count > 0) productsQuery = productsQuery.Where(p => query.BreweryList.Contains(p.Brewery));
        if (query.CountryList.Count > 0) productsQuery = productsQuery.Where(p => query.CountryList.Contains(p.Country));

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
            Page = page,
            Size = size
        };
        await context.RaiseEvent(sortRequest);

        var pageIds = sortRequest.OrderedIds?.ToList()
            ?? await productsQuery
                .OrderBy(p => p.Name)
                .Skip((page - 1) * size)
                .Take(size)
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

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)size);
        var responseModel = request.GetComposedResponseModel();

        // Drive the response array from the explicit ordered key list
        // rather than productsModel.Values — Dictionary enumeration
        // order is documented as unspecified and must not be the
        // source of truth for a user-visible sort.
        responseModel.Products = pageIds
            .Where(productsModel.ContainsKey)
            .Select(id => productsModel[id])
            .ToList();
        responseModel.Page = page;
        responseModel.PageSize = size;
        responseModel.TotalCount = totalCount;
        responseModel.TotalPages = totalPages;
    }
}
