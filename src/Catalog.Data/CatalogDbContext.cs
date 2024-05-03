using ITOps.Shared;

namespace Catalog.Data;

public class CatalogDbContext : LiteDbContext
{
    public CatalogDbContext(LiteDbOptions options) : base(options)
    {
    }
}