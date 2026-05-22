using Catalog.Data;
using Catalog.Data.Seed;
using Catalog.Endpoint.Handlers;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string EndpointName = "Catalog";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

// One SQLite file per service: domain data and the NServiceBus
// persister tables (saga, outbox, subscriptions) share the same
// connection string so a future TransactionalSession can commit
// across both atomically.
var sqliteConnectionString = SqliteStorage.GetConnectionString("catalog");

// SQLite via EF Core. AddDbContext registers the context as scoped,
// which is what message handlers and request handlers expect (each
// message / request gets its own change-tracker).
hostBuilder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlite(sqliteConnectionString));

// Configure NServiceBus
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure(sqliteConnectionString);

// Explicit handler registration (NServiceBus 10.2+). The call is
// intercepted by the source generator, so convention-based handlers
// (no IHandleMessages<T>) are wired up at compile time rather than
// via reflection-based assembly scanning.
endpointConfiguration.AddHandler<CompleteOrderHandler>();

// AddNServiceBusEndpoint (NServiceBus 10.2+) replaces UseNServiceBus from
// NServiceBus.Extensions.Hosting. It is the multi-host-aware registration
// surface; called without an identifier here, it behaves as a single-endpoint
// host with standard (non-keyed) DI registration.
hostBuilder.Services.AddNServiceBusEndpoint(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

// Apply migrations and seed before the host starts processing messages.
// The endpoint is the only process that seeds; the gateway just queries.
using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext);
}

await host.RunAsync();
