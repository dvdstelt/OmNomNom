using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.ServiceComposition.Helpers;

namespace Shipping.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddDbContext<ShippingDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("shipping")));
        options.Services.AddSingleton<CacheHelper>();
    }
}
