using System.Dynamic;
using Catalog.Data.Models;

namespace Catalog.ServiceComposition.Email;

// Email-summary viewmodel: one entry per ordered line, carrying the
// product's display fields (Name, Description, ImageUrl) alongside
// the quantity that was actually ordered. Source rows come from the
// Order -> OrderItem -> Product join.
public static class Mapper
{
    public static IDictionary<Guid, dynamic> MapToDictionary(IEnumerable<(OrderItem OrderItem, Product Product)> rows)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var row in rows)
        {
            productsViewModel[row.OrderItem.ProductId] = MapToViewModel(row.OrderItem, row.Product);
        }

        return productsViewModel;
    }

    public static dynamic MapToViewModel(OrderItem orderedProduct, Product product)
    {
        dynamic vm = new ExpandoObject();
        vm.ProductId = orderedProduct.ProductId;
        vm.Name = product.Name;
        vm.Description = product.Description;
        vm.ImageUrl = product.ImageUrl;
        vm.Quantity = orderedProduct.Quantity;
        // Default; Finance flips this to false for items it could not
        // fulfil, in OnOrderEmailProductsLoaded.
        vm.Fulfilled = true;
        return vm;
    }
}
