using System.Dynamic;
using Catalog.Data;
using Catalog.Data.Models;
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

        var productsModel = new Dictionary<Guid, dynamic>();
        foreach (var row in rows)
        {
            productsModel[row.OrderItem.ProductId] = MapToViewModel(row.OrderItem, row.Product);
        }

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductsLoaded
        {
            Products = productsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.Products = productsModel.Values.ToList();
        vm.OrderId = orderId;
    }

    private dynamic MapToViewModel(OrderItem orderedProduct, Product product)
    {
        dynamic vm = new ExpandoObject();
        vm.ProductId = orderedProduct.ProductId;
        vm.Name = product.Name;
        vm.Description = product.Description;
        vm.ImageUrl = product.ImageUrl;
        vm.Quantity = orderedProduct.Quantity;
        return vm;
    }
}
