using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentInfo.Data;
using PaymentInfo.Data.Seed;
using PaymentInfo.Endpoint.Handlers;

namespace Microsoft.Extensions.DependencyInjection;

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

        var endpointConfiguration = new NServiceBus.EndpointConfiguration("PaymentInfo");
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString);
        endpointConfiguration.AddHandler<SubmitPaymentInfoHandler>();

        services.AddNServiceBusEndpoint(endpointConfiguration, "PaymentInfo");
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
