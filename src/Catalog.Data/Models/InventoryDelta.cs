namespace Catalog.Data.Models;

public class InventoryDelta
{
    public Guid ProductId { get; set; }
    public int Delta { get; set; }
    public DateTime TimeStamp { get; set; }
}