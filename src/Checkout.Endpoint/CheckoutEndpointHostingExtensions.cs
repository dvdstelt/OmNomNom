using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Messaging.Persistence.Sqlite.TransactionalSession;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

// Checkout has no business handlers - it is the processor for the gateway's
// TransactionalSession. It receives the control message the gateway sends on
// commit and dispatches the queued outbox messages from checkout.db. The
// assembly scanner is disabled because, in AllInOne, the load context has
// every other endpoint's [Handler]-marked types in it; we don't want any of
// them swept into Checkout's configuration. No database seeder is needed:
// checkout.db only holds NServiceBus persistence and TransactionalSession
// tables, and the NServiceBus installers create those at startup.
public static class CheckoutEndpointHostingExtensions
{
    public static IServiceCollection AddCheckoutEndpoint(this IServiceCollection services)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("checkout");

        var endpointConfiguration = new NServiceBus.EndpointConfiguration("Checkout");
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString,
            configurePersistence: persistence => persistence.EnableTransactionalSession());

        services.AddNServiceBusEndpoint(endpointConfiguration, "Checkout");
        return services;
    }
}
