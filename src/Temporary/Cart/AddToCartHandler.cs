using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Temporary.Caching;

namespace OmNomNom.Website.ViewModelComposition;

public class AddToCartHandler : ICompositionRequestsHandler
{
    readonly CacheHelper cacheHelper;

    public AddToCartHandler(CacheHelper cacheHelper)
    {
        this.cacheHelper = cacheHelper;
    }

    [HttpPost("/cart/addproduct/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var productToAdd = await request.Bind<ProductModel>();

        // Using the orderIdString we'll create a proper Guid and try to retrieve a potentially existing order
        var orderId = (productToAdd.orderId.ToString() == new Guid().ToString()) ? Guid.NewGuid() : productToAdd.orderId;

        var cart = await cacheHelper.GetCart(orderId);

        var existingCartItem = cart?.Items.SingleOrDefault(s => s.ProductId == productToAdd.Detail.Id);
        if (existingCartItem != null)
            existingCartItem.Quantity += productToAdd.Detail.Quantity;
        else
        {
            var newCartItem = new CartItem()
                { ProductId = productToAdd.Detail.Id, Quantity = productToAdd.Detail.Quantity };

            cart?.Items.Add(newCartItem);
        }

        await cacheHelper.StoreCart(cart);

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
    }
}