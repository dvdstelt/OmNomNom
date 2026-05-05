namespace Catalog.Data.Models;

public class InventoryDelta
{
    // Surrogate PK; this table is an append-only log so the same
    // ProductId appears on multiple rows.
    public int Id { get; set; }
    public Guid ProductId { get; set; }
    public int Delta { get; set; }
    public DateTime TimeStamp { get; set; }
}