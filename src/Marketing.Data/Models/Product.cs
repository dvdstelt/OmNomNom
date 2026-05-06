namespace Marketing.Data.Models;

public class Product
{
    public Guid ProductId { get; set; }
    public double Rating { get; set; }
    public int RatingCount { get; set; }
}