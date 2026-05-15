using Finance.Data;
using Finance.Endpoint.Messages.Commands;
using Finance.ServiceComposition.Helpers;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization, IConfigureEndpointRouting
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddDbContext<FinanceDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("finance")));
        options.Services.AddScoped<OrderSubtotalReader>();
    }

    public void ConfigureRouting(RoutingSettings<LearningTransport> routing)
    {
        routing.RouteToEndpoint(typeof(SubmitOrderItems).Assembly, "Finance");
    }
}
