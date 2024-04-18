namespace Finance.Data.Models;

public class Product
{
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
}