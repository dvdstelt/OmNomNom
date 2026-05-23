using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Data.Seed;
using Shipping.Endpoint.Handlers;
using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Sagas;

namespace Microsoft.Extensions.DependencyInjection;

// Shared registration entry point - see CatalogEndpointHostingExtensions
// for the rationale behind the disabled assembly scanner and the
// hosted database seeder.
public static class ShippingEndpointHostingExtensions
{
    public static IServiceCollection AddShippingEndpoint(this IServiceCollection services)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("shipping");

        services.AddDbContext<ShippingDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        services.AddHostedService<ShippingDatabaseSeeder>();

        var endpointConfiguration = new EndpointConfiguration("Shipping");
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

        services.AddNServiceBusEndpoint(endpointConfiguration, endpointConfiguration.EndpointName);
        return services;
    }
}

sealed class ShippingDatabaseSeeder(IServiceProvider services) : DatabaseSeederHostedService(services)
{
    protected override async Task SeedAsync(IServiceProvider scopedServices, CancellationToken cancellationToken)
    {
        var dbContext = scopedServices.GetRequiredService<ShippingDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext, cancellationToken);
    }
}
