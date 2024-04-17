using Microsoft.AspNetCore.Mvc;

namespace OmNomNom.Website.ViewModelComposition;

class AddressModel
{
    [FromRoute] public Guid orderId { get; set; }
    [FromBody] public Dictionary<string, Address> Body { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string Town { get; set; }
    public string Country { get; set; }
}

public class ProductModel
{
    [FromRoute] public Guid orderId { get; set; }
    [FromBody] public ProductDetail Detail { get; set; }
}

public class ProductDetail
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}

public class Product
{
    public Guid Id { get; internal set; }
    public string UrlId { get; internal set; }
    public string Name { get; internal set; }
    public string Image { get; internal set; }
    public decimal Price { get; internal set; }
    public double Stars { get; internal set; }
    public string Category { get; internal set; }
    public int InStock { get; internal set; }
    public int ReviewCount { get; internal set; }
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