using System.Dynamic;
using Catalog.Data.Models;

namespace Catalog.ServiceComposition.Products;

public static class Mapper
{
    public static dynamic MapToViewModel(Product product, int inStock)
    {
        dynamic vm = new ExpandoObject();
        vm.ProductId = product.ProductId;
        vm.Name = product.Name;
        vm.Description = product.Description;
        vm.ImageUrl = product.ImageUrl;
        vm.BeerStyle = product.BeerStyle;
        vm.Brewery = product.Brewery;
        vm.Country = product.Country;
        vm.InStock = inStock;
        return vm;
    }
}
