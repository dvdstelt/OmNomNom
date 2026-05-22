using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shipping.Data;
using Shipping.Data.Seed;
using Shipping.Endpoint.Handlers;
using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Sagas;

namespace Shipping.Endpoint;

// Shared registration entry point - see CatalogEndpointHost for the
// rationale behind the disabled assembly scanner.
public static class ShippingEndpointHost
{
    const string EndpointName = "Shipping";

    public static void Register(IHostApplicationBuilder builder)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("shipping");

        builder.Services.AddDbContext<ShippingDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString, configureRouting: routing =>
        {
            routing.RouteToEndpoint(typeof(ShipOrderRequest).Assembly, "Shipping");
        });
        endpointConfiguration.AddHandler<SubmitDeliveryOptionHandler>();
        endpointConfiguration.AddHandler<SubmitShippingAddressHandler>();
        endpointConfiguration.AddHandler<ShipOrderRequestHandler>();
        endpointConfiguration.AddSaga<ShippingPolicy>();
        endpointConfiguration.AddSaga<ReturnPolicy>();

        builder.Services.AddNServiceBusEndpoint(endpointConfiguration);
    }

    public static async Task InitializeDatabaseAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShippingDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext);
    }
}
