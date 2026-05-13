using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.Endpoint.Messages.Commands;

namespace Shipping.ServiceComposition;

public class Startup : IViewModelCompositionOptionsCustomization, IConfigureEndpointRouting
{
    public void Customize(ViewModelCompositionOptions options)
    {
        options.Services.AddDbContext<ShippingDbContext>(opts =>
            opts.UseSqlite(SqliteStorage.GetConnectionString("shipping")));
    }

    public void ConfigureRouting(RoutingSettings<LearningTransport> routing)
    {
        routing.RouteToEndpoint(typeof(SubmitDeliveryOption).Assembly, "Shipping");
    }
}
