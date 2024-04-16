using Microsoft.AspNetCore.Mvc;

namespace OmNomNom.Website.ViewModelComposition;

class AddressModel
{
    [FromRoute] public Guid orderId { get; set; }
    [FromBody] public Dictionary<string, Address> Body { get; set; }
}

class Address
{
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string Town { get; set; }
    public string Country { get; set; }
}

public class UserCart
{
    public const string SessionKeyName = "UserCart";

    public Guid OrderId { get; set; }
    public List<CartItem> Items { get; set; } = new();
}

public class CartItem
{
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
}