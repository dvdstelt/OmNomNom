using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentInfo.Data;
using PaymentInfo.Data.Seed;

namespace PaymentInfo.Endpoint;

// Shared registration entry point - see CatalogEndpointHostingExtensions
// for the rationale behind the disabled assembly scanner and the
// hosted database seeder.
public static class PaymentInfoEndpointHostingExtensions
{
    public static IServiceCollection AddPaymentInfoEndpoint(this IServiceCollection services)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("paymentinfo");

        services.AddDbContext<PaymentInfoDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        services.AddHostedService<PaymentInfoDatabaseSeeder>();

        var endpointConfiguration = new EndpointConfiguration("PaymentInfo");
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString);

        endpointConfiguration.Handlers.PaymentInfo.AddAll();

        services.AddNServiceBusEndpoint(endpointConfiguration, endpointConfiguration.EndpointName);
        return services;
    }
}

sealed class PaymentInfoDatabaseSeeder(IServiceProvider services) : DatabaseSeederHostedService(services)
{
    protected override async Task SeedAsync(IServiceProvider scopedServices, CancellationToken cancellationToken)
    {
        var dbContext = scopedServices.GetRequiredService<PaymentInfoDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext, cancellationToken);
    }
}
