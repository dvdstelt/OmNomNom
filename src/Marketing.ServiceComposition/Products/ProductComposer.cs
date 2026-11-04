using Marketing.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Marketing.ServiceComposition.Products;

[CompositionHandler]
public class ProductComposer(MarketingDbContext dbContext, IHttpContextAccessor http)
{
    // The product id comes from the route, so there is no need to wait
    // for Catalog. This runs side by side with the other boundaries'
    // composers and writes only Marketing's own slice of the response.
    [HttpGet("/product/{productId}")]
    public async Task Handle(Guid productId)
    {
        var request = http.HttpContext!.Request;
        var vm = request.GetComposedResponseModel();
        var ct = request.HttpContext.RequestAborted;

        var product = await dbContext.Products
            .SingleAsync(s => s.ProductId == productId, ct);

        vm.Popularity = new
        {
            product.Rating,
            product.RatingCount,
            product.OrderCount,
            product.Trending
        };
    }
}
