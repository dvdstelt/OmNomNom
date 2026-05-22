using Marketing.Endpoint;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);
MarketingEndpointHost.Register(hostBuilder);

var host = hostBuilder.Build();
Console.Title = host.Services.GetRequiredService<IHostEnvironment>().ApplicationName;

await MarketingEndpointHost.InitializeDatabaseAsync(host);
await host.RunAsync();
