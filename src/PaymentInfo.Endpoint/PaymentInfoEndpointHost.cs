using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentInfo.Data;
using PaymentInfo.Data.Seed;
using PaymentInfo.Endpoint.Handlers;

namespace PaymentInfo.Endpoint;

// Shared registration entry point - see CatalogEndpointHost for the
// rationale behind the disabled assembly scanner.
public static class PaymentInfoEndpointHost
{
    const string EndpointName = "PaymentInfo";

    public static void Register(IHostApplicationBuilder builder)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("paymentinfo");

        builder.Services.AddDbContext<PaymentInfoDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString);
        endpointConfiguration.AddHandler<SubmitPaymentInfoHandler>();

        builder.Services.AddNServiceBusEndpoint(endpointConfiguration);
    }

    public static async Task InitializeDatabaseAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentInfoDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext);
    }
}
