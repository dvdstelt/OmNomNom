using ITOps.Shared;
using Marketing.Data;
using Marketing.Data.Migrations;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace Marketing.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddSingleton<MarketingDbContext>(provider =>
        {
            var dbOptions = new LiteDbOptions("shipping", DatabaseInitializer.Initialize);
            return new MarketingDbContext(dbOptions);
        });

    }
}