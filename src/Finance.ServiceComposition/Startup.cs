using Finance.Data;
using Finance.Data.Migrations;
using Finance.ServiceComposition.Helpers;
using ITOps.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddSingleton<FinanceDbContext>(provider =>
        {
            var liteDbOptions = new LiteDbOptions("finance", DatabaseInitializer.Initialize);
            return new FinanceDbContext(liteDbOptions);
        });
        options.Services.AddSingleton<CacheHelper>();
    }
}