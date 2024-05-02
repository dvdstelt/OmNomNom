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

var ms = host.Services.GetService<IMessageSession>();
var @event = new OrderShipped();
@event.OrderId = Guid.Parse("08bebbee-0e7e-4368-afab-74f4720f5f4e");
await ms.Publish(@event);

await host.RunAsync();