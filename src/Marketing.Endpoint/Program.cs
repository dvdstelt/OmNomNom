using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Marketing.Data;
using Marketing.Data.Seed;
using Marketing.Endpoint;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string EndpointName = "Marketing";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddDbContext<MarketingDbContext>(options =>
    options.UseSqlite(SqliteStorage.GetConnectionString("marketing")));

// Configure NServiceBus. Marketing only subscribes to events, so no
// command routing is needed - LearningTransport's pub/sub handles the
// rest via the shared .learningtransport folder.
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure();
hostBuilder.UseNServiceBus(endpointConfiguration);

// Background recompute that keeps Marketing.Product.Trending honest as
// seeded events age out of the 30-day window.
hostBuilder.Services.AddHostedService<TrendingRecomputeService>();

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MarketingDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext);
}

await host.RunAsync();
