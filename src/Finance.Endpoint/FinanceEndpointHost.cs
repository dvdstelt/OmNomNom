using Finance.Data;
using Finance.Data.Seed;
using Finance.Endpoint.Handlers;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Finance.Endpoint;

// Shared registration entry point - see CatalogEndpointHost for the
// rationale behind the disabled assembly scanner.
public static class FinanceEndpointHost
{
    const string EndpointName = "Finance";

    public static void Register(IHostApplicationBuilder builder)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("finance");

        builder.Services.AddDbContext<FinanceDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString);
        endpointConfiguration.AddHandler<OrderCancelledHandler>();
        endpointConfiguration.AddHandler<OrderPlacedHandler>();
        endpointConfiguration.AddHandler<SubmitBillingAddressHandler>();
        endpointConfiguration.AddHandler<SubmitDeliveryOptionHandler>();
        endpointConfiguration.AddHandler<SubmitOrderItemsHandler>();

        builder.Services.AddNServiceBusEndpoint(endpointConfiguration, EndpointName);
    }

    public static async Task InitializeDatabaseAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext);
    }
}
