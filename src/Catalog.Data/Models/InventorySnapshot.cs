namespace Catalog.Data.Models;

public class InventorySnapshot
{
    public Guid ProductId { get; set; }
    public int EstimatedInStock { get; set; }
}