using Catalog.Data;
using Catalog.Data.Seed;
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

// CompleteOrder and SubmitOrderItems both arrive at this endpoint
// from a single workflow submit. NServiceBus does not guarantee
// message ordering, so CompleteOrder might be processed before
// SubmitOrderItems has written the Order row. Override the shared
// "no delayed retries" default with a small backoff so
// CompleteOrderHandler waits for the row instead of dead-lettering.
endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(3));

hostBuilder.UseNServiceBus(endpointConfiguration);

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
