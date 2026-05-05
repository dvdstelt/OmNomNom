using Catalog.Data;
using Catalog.Data.Seed;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string EndpointName = "Catalog";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

// SQLite via EF Core. AddDbContext registers the context as scoped,
// which is what message handlers and request handlers expect (each
// message / request gets its own change-tracker).
hostBuilder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlite(SqliteStorage.GetConnectionString("catalog")));

// Configure NServiceBus
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure();
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
