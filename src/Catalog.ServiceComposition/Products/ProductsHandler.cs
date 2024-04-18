using System.Dynamic;
using Microsoft.AspNetCore.Http;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

public class ProductsHandler : ICompositionRequestsHandler
{
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();


    }

    IDictionary<int, dynamic> MapToDictionary(IEnumerable<Guid> availableProducts)
    {
        var availableProductsModel = new Dictionary<Guid, dynamic>();

        foreach (var id in availableProducts)
        {
            dynamic vm = new ExpandoObject();
            vm.ProductId = productId;

            availableProductsModel[id] = vm;
        }

        return availableProductsModel;
    }
}