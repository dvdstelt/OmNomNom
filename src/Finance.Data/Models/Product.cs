namespace Finance.Data.Models;

public class Product
{
    public Guid PriceId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public DateTime ValidSince { get; set; }
}