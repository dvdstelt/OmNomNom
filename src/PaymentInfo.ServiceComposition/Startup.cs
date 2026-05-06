using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentInfo.Data;
using PaymentInfo.ServiceComposition.Helpers;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddDbContext<PaymentInfoDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("paymentinfo")));
        options.Services.AddSingleton<CacheHelper>();
    }
}
