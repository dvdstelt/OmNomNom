using Finance.Data;
using Finance.Data.Seed;
using Finance.Endpoint;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

// Shared registration entry point - see CatalogEndpointHostingExtensions
// for the rationale behind the disabled assembly scanner and the
// hosted database seeder.
public static class FinanceEndpointHostingExtensions
{
    public static IServiceCollection AddFinanceEndpoint(this IServiceCollection services)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("finance");

        services.AddDbContext<FinanceDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        services.AddHostedService<FinanceDatabaseSeeder>();

        var endpointConfiguration = new EndpointConfiguration("Finance");
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString);

        endpointConfiguration.Handlers.Finance.AddAll();

        services.AddNServiceBusEndpoint(endpointConfiguration, endpointConfiguration.EndpointName);
        return services;
    }
}

sealed class FinanceDatabaseSeeder(IServiceProvider services) : DatabaseSeederHostedService(services)
{
    protected override async Task SeedAsync(IServiceProvider scopedServices, CancellationToken cancellationToken)
    {
        var dbContext = scopedServices.GetRequiredService<FinanceDbContext>();
        await DatabaseInitializer.InitializeAsync(dbContext, cancellationToken);
    }
}
