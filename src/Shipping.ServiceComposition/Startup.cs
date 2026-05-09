using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;
using Shipping.Data;

namespace Shipping.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddDbContext<ShippingDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("shipping")));
    }
}
