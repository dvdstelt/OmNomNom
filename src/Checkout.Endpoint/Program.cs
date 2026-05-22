using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Messaging.Persistence.Sqlite.TransactionalSession;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string EndpointName = "Checkout";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

// The Checkout endpoint is the processor for the gateway's
// TransactionalSession. It owns no business handlers - the only
// thing it does is receive the control message the gateway sends
// on commit and dispatch the queued outbox messages from
// checkout.db. Bound to the same SQLite file the gateway and the
// IWorkflowStore use; outbox + transactional session feature must
// be on, but no ProcessorEndpoint is set because this endpoint
// IS the processor.
var sqliteConnectionString = SqliteStorage.GetConnectionString("checkout");

var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure(sqliteConnectionString,
    configurePersistence: persistence => persistence.EnableTransactionalSession());
// Checkout has no business handlers - it only dispatches the outbox
// messages queued by the CompositionGateway's TransactionalSession on
// commit. No AddHandler<>() calls needed; assembly scanning still
// picks up nothing handler-shaped in this assembly.
hostBuilder.Services.AddNServiceBusEndpoint(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

await host.RunAsync();
