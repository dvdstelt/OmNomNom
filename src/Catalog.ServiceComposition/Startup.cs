using Catalog.Data;
using Catalog.Data.Migrations;
using Catalog.ServiceComposition.Helpers;
using ITOps.Shared;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddSingleton<CatalogDbContext>(provider =>
        {
            var dbOptions = new LiteDbOptions("catalog", DatabaseInitializer.Initialize);
            return new CatalogDbContext(dbOptions);
        });
        options.Services.AddSingleton<CacheHelper>();
    }
}