using Catalog.Data.Migrations;
using Finance.Data;
using ITOps.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition;

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
        // options.Services.Configure<LiteDbOptions>(Configuration.GetSection("LiteDbOptionsFinance"))
        //     .Configure<LiteDbOptions>(s =>
        //     {
        //         s.DatabaseName = "finance";
        //         s.DatabaseInitializer = DatabaseInitializer.Initialize;
        //     });
        options.Services.AddSingleton<FinanceDbContext>(provider => new FinanceDbContext(new LiteDbOptions("finance", DatabaseInitializer.Initialize)));
    }
}