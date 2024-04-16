using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class AddToCart : ICompositionRequestsHandler
{
    [HttpPost("/cart/addproduct/{productId}")]
    public async Task Handle(HttpRequest request)
    {
        // Get cart from session state
        var value = request.HttpContext.Session.GetString(UserCart.SessionKeyName);
        var cart = value == null ? new() : JsonSerializer.Deserialize<UserCart>(value);

        // Bind submitted data to CartItem
        // var newCartItem = await request.Bind<CartItem>();
        var newCartItem = new CartItem()
        {
            ProductId = Guid.Parse("cc0981ac-6075-48f5-ad1a-7cae81f2c366"),
            Name = "Pizza",
            Price = 5m,
            Quantity = 2
        };

        var existingCartItem = cart?.Items.SingleOrDefault(s => s.ProductId == newCartItem.ProductId);
        if (existingCartItem != null)
            existingCartItem.Quantity += newCartItem.Quantity;
        else
        {
            cart?.Items.Add(newCartItem);
        }

        var serialize = JsonSerializer.Serialize(cart);
        request.HttpContext.Session.SetString(UserCart.SessionKeyName, serialize);
    }
}