using Catalog.Data;
using Catalog.Endpoint.Messages.Commands;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization, IConfigureEndpointRouting
{
    public void Customize(ViewModelCompositionOptions options)
    {
        // Read-only access to the catalog domain DB from the gateway.
        // Schema creation and seeding are owned by Catalog.Endpoint.
        // Cart state lives in the workflow store, not in this DB.
        options.Services.AddDbContext<CatalogDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("catalog")));
    }

    public void ConfigureRouting(RoutingSettings<LearningTransport> routing)
    {
        routing.RouteToEndpoint(typeof(CompleteOrder).Assembly, "Catalog");
    }
}
