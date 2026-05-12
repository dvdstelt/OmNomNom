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
            dynamic vm = new ExpandoObject();
            vm.ProductId = line.ProductId;
            vm.Quantity = line.Quantity;

            var matchingProduct = products.Single(p => p.ProductId == line.ProductId);
            vm.Name = matchingProduct.Name;
            vm.ImageUrl = matchingProduct.ImageUrl;

            productsViewModel[line.ProductId] = vm;
        }

        return productsViewModel;
    }
}
