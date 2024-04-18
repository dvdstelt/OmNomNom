using Catalog.Data;
using Catalog.Data.Migrations;
using ITOps.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    // public Startup(IConfiguration configuration)
    // {
    //     Configuration = configuration;
    // }
    //
    // IConfiguration Configuration { get; }

    public void Customize(ViewModelCompositionOptions options)
    {
        // options.Services//.Configure<LiteDbOptions>(Configuration.GetSection("LiteDbOptionsCatalog"))
        //     .Configure<LiteDbOptions>(s =>
        //     {
        //         s.DatabaseName = "catalog";
        //         s.DatabaseInitializer = DatabaseInitializer.Initialize;
        //     });
        options.Services.AddSingleton<CatalogDbContext>(provider => new CatalogDbContext(new LiteDbOptions("catalog", DatabaseInitializer.Initialize)));
    }
}