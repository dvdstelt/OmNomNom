using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shipping.Endpoint;

var hostBuilder = Host.CreateApplicationBuilder(args);
ShippingEndpointHost.Register(hostBuilder);

var host = hostBuilder.Build();
Console.Title = host.Services.GetRequiredService<IHostEnvironment>().ApplicationName;

await ShippingEndpointHost.InitializeDatabaseAsync(host);
await host.RunAsync();
