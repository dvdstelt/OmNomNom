using System.Dynamic;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Workflow;

namespace Catalog.ServiceComposition.ShoppingCart;

public static class Mapper
{
    public static IDictionary<Guid, dynamic> MapToDictionary(CartSlice cart, List<Product> products)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var line in cart.Items)
        {
            var matchingProduct = products.Single(p => p.ProductId == line.ProductId);
            productsViewModel[line.ProductId] = MapToViewModel(line, matchingProduct);
        }

        return productsViewModel;
    }

    public static dynamic MapToViewModel(CartLine line, Product product)
    {
        dynamic vm = new ExpandoObject();
        vm.ProductId = line.ProductId;
        vm.Quantity = line.Quantity;
        vm.Name = product.Name;
        vm.ImageUrl = product.ImageUrl;
        return vm;
    }
}
