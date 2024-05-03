using ITOps.Shared;
using ITOps.Shared.EndpointConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shipping.Data;
using Shipping.Data.Migrations;
using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Messages.Events;

const string EndpointName = "Shipping";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

// Configure LiteDb
hostBuilder.Services.AddSingleton<ShippingDbContext>(provider =>
{
    var dbOptions = new LiteDbOptions("shipping", DatabaseInitializer.Initialize);
    return new ShippingDbContext(dbOptions);
});

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

await host.RunAsync();