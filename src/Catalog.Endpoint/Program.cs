using Catalog.Endpoint;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);
CatalogEndpointHost.Register(hostBuilder);

var host = hostBuilder.Build();
Console.Title = host.Services.GetRequiredService<IHostEnvironment>().ApplicationName;

await CatalogEndpointHost.InitializeDatabaseAsync(host);
await host.RunAsync();
