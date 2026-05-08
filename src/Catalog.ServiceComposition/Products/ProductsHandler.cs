using Catalog.Data;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Events;
using Marketing.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

// Server-side product search. Catalog owns the result set and applies
// filters on Catalog-owned fields (category/brewery/country/in-stock).
// Sorting on Marketing-owned signals (rating/orderCount/trending) is
// delegated to IProductRanker so Catalog never reads Marketing's
// database directly.
//
// Query parameters:
//   categories  comma-separated; empty -> no filter
//   breweries   comma-separated; empty -> no filter
//   countries   comma-separated; empty -> no filter
//   inStock     bool, default true (0-stock items hidden)
//   sort        default | rating | orderCount | trending
//   page        int >= 1, default 1
//   size        int 1-MaxPageSize, default DefaultPageSize
//
// The composed response carries:
//   Products    list of dictionary values for the page (in order)
//   Page, PageSize, TotalCount, TotalPages
public class ProductsHandler(CatalogDbContext dbContext, IProductRanker ranker) : ICompositionRequestsHandler
{
    const int DefaultPageSize = 12;
    const int MaxPageSize = 50;

    [HttpGet("/products")]
    public async Task Handle(HttpRequest request)
    {
        var ct = request.HttpContext.RequestAborted;
        var query = request.Query;

        var categories = ReadList(query, "categories");
        var breweries = ReadList(query, "breweries");
        var countries = ReadList(query, "countries");
        var inStock = ReadBool(query, "inStock", defaultValue: true);
        var sort = ReadSort(query);
        var page = Math.Max(1, ReadInt(query, "page", defaultValue: 1));
        var size = Math.Clamp(ReadInt(query, "size", defaultValue: DefaultPageSize), 1, MaxPageSize);

        // 1. Filter Catalog-owned attributes. Each filter only applies
        //    when the caller supplied at least one value.
        IQueryable<Product> productsQuery = dbContext.Products;
        if (categories.Count > 0) productsQuery = productsQuery.Where(p => categories.Contains(p.Category));
        if (breweries.Count > 0) productsQuery = productsQuery.Where(p => breweries.Contains(p.Brewery));
        if (countries.Count > 0) productsQuery = productsQuery.Where(p => countries.Contains(p.Country));

        // 2. Apply the in-stock filter via a join on the snapshot.
        //    The default of true keeps zero-stock items off the list,
        //    matching what users would expect on a storefront.
        if (inStock)
        {
            productsQuery =
                from p in productsQuery
                join s in dbContext.InventorySnapshots on p.ProductId equals s.ProductId
                where s.EstimatedInStock > 0
                select p;
        }

        // 3. Materialise the filtered candidate set as IDs only. We
        //    re-query the full Product rows for just the page slice
        //    after the sort decides the order.
        var candidateIds = await productsQuery.Select(p => p.ProductId).ToListAsync(ct);
        var totalCount = candidateIds.Count;

        // 4. Sort. Default sorts by Name in Catalog; the others ask
        //    Marketing for the order.
        IReadOnlyList<Guid> orderedIds;
        if (sort is null)
        {
            orderedIds = await dbContext.Products
                .Where(p => candidateIds.Contains(p.ProductId))
                .OrderBy(p => p.Name)
                .Select(p => p.ProductId)
                .ToListAsync(ct);
        }
        else
        {
            orderedIds = await ranker.RankAsync(candidateIds, sort.Value, ct);
        }

        var pageIds = orderedIds.Skip((page - 1) * size).Take(size).ToList();

        // 5. Load the Product + InventorySnapshot rows for the page,
        //    preserving the rank order from step 4.
        var pageRows = await (
            from p in dbContext.Products
            join s in dbContext.InventorySnapshots on p.ProductId equals s.ProductId
            where pageIds.Contains(p.ProductId)
            select new { Product = p, Inventory = s }).ToListAsync(ct);

        var lookup = pageRows.ToDictionary(x => x.Product.ProductId);
        var orderedPage = pageIds
            .Where(lookup.ContainsKey)
            .Select(id => lookup[id])
            .ToList();

        var productsModel = new Dictionary<Guid, dynamic>();
        foreach (var row in orderedPage)
        {
            var vm = Mapper.MapToViewModel(row.Product, row.Inventory);
            productsModel[row.Product.ProductId] = vm;
        }

        // 6. Compose: Marketing/Finance subscribers attach their
        //    slices to the page items (not the whole catalog).
        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductsLoaded
        {
            Products = productsModel
        });

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)size);
        var responseModel = request.GetComposedResponseModel();
        responseModel.Products = productsModel.Values.ToList();
        responseModel.Page = page;
        responseModel.PageSize = size;
        responseModel.TotalCount = totalCount;
        responseModel.TotalPages = totalPages;
    }

    static List<string> ReadList(IQueryCollection query, string name)
    {
        if (!query.TryGetValue(name, out var values))
            return [];

        return values
            .SelectMany(v => (v ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }

    static bool ReadBool(IQueryCollection query, string name, bool defaultValue)
    {
        if (!query.TryGetValue(name, out var values) || values.Count == 0) return defaultValue;
        return bool.TryParse(values[0], out var parsed) ? parsed : defaultValue;
    }

    static int ReadInt(IQueryCollection query, string name, int defaultValue)
    {
        if (!query.TryGetValue(name, out var values) || values.Count == 0) return defaultValue;
        return int.TryParse(values[0], out var parsed) ? parsed : defaultValue;
    }

    static ProductRankBy? ReadSort(IQueryCollection query)
    {
        if (!query.TryGetValue("sort", out var values) || values.Count == 0)
            return null;

        return values[0]?.ToLowerInvariant() switch
        {
            "rating" => ProductRankBy.Rating,
            "ordercount" => ProductRankBy.OrderCount,
            "trending" => ProductRankBy.Trending,
            _ => null
        };
    }
}
