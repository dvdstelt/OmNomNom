using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shipping.Data;
using Shipping.Data.Seed;
using Shipping.Endpoint.Handlers;
using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Messages.Events;
using Shipping.Endpoint.Sagas;

const string EndpointName = "Shipping";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

var sqliteConnectionString = SqliteStorage.GetConnectionString("shipping");

hostBuilder.Services.AddDbContext<ShippingDbContext>(options =>
    options.UseSqlite(sqliteConnectionString));

// Configure NServiceBus
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure(sqliteConnectionString, configureRouting: routing =>
{
    routing.RouteToEndpoint(typeof(ShipOrderRequest).Assembly, "Shipping");
});
endpointConfiguration.AddHandler<SubmitDeliveryOptionHandler>();
endpointConfiguration.AddHandler<SubmitShippingAddressHandler>();
endpointConfiguration.AddHandler<ShipOrderRequestHandler>();
endpointConfiguration.AddSaga<ShippingPolicy>();
endpointConfiguration.AddSaga<ReturnPolicy>();
hostBuilder.Services.AddNServiceBusEndpoint(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShippingDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext);
}

await host.RunAsync();
