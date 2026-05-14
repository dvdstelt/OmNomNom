using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Email;

[CompositionHandler]
public class OrderSummaryComposer(CatalogDbContext dbContext, IHttpContextAccessor http)
{
    [HttpGet("/email/summary/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        var rows = await (
            from o in dbContext.Orders
            from i in o.Products
            join p in dbContext.Products on i.ProductId equals p.ProductId
            where o.OrderId == orderId
            select new { OrderItem = i, Product = p }).ToListAsync(ct);

        var productsModel = Mapper.MapToDictionary(rows.Select(r => (r.OrderItem, r.Product)));

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductsLoaded
        {
            Products = productsModel
        });
        // Email-specific second pass so per-order state (e.g. Fulfilled
        // from Finance) can attach without polluting the generic
        // ProductsLoaded contract.
        await context.RaiseEvent(new OrderEmailProductsLoaded
        {
            OrderId = orderId,
            Products = productsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.Products = productsModel.Values.ToList();
        vm.OrderId = orderId;
    }
}
