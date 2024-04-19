namespace Marketing.Data.Models;

public class Product
{
    public Guid ProductId { get; set; }
    public double Stars { get; set; }
    public int ReviewCount { get; set; }
}