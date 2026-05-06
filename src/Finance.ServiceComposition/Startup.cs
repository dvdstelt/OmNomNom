using Finance.Data;
using Finance.ServiceComposition.Helpers;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddDbContext<FinanceDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("finance")));
        options.Services.AddSingleton<CacheHelper>();
    }
}
