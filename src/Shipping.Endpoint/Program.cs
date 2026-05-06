using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shipping.Data;
using Shipping.Data.Seed;
using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Messages.Events;

const string EndpointName = "Shipping";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddDbContext<ShippingDbContext>(options =>
    options.UseSqlite(SqliteStorage.GetConnectionString("shipping")));

// Configure NServiceBus
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure(routing =>
{
    routing.RouteToEndpoint(typeof(ShipOrderRequest).Assembly, "Shipping");
});
hostBuilder.UseNServiceBus(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShippingDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext);
}

await host.RunAsync();
