using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Messaging.Persistence.Sqlite.TransactionalSession;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Checkout.Endpoint;

// Checkout has no business handlers - it is the processor for the gateway's
// TransactionalSession. It receives the control message the gateway sends on
// commit and dispatches the queued outbox messages from checkout.db. The
// assembly scanner is disabled because, in AllInOne, the load context has
// every other endpoint's [Handler]-marked types in it; we don't want any of
// them swept into Checkout's configuration.
public static class CheckoutEndpointHost
{
    const string EndpointName = "Checkout";

    public static void Register(IHostApplicationBuilder builder)
    {
        var sqliteConnectionString = SqliteStorage.GetConnectionString("checkout");

        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.AssemblyScanner().Disable = true;
        endpointConfiguration.Configure(sqliteConnectionString,
            configurePersistence: persistence => persistence.EnableTransactionalSession());

        builder.Services.AddNServiceBusEndpoint(endpointConfiguration, EndpointName);
    }
}
