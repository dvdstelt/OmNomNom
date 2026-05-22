using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentInfo.Endpoint;

var hostBuilder = Host.CreateApplicationBuilder(args);
PaymentInfoEndpointHost.Register(hostBuilder);

var host = hostBuilder.Build();
Console.Title = host.Services.GetRequiredService<IHostEnvironment>().ApplicationName;

await PaymentInfoEndpointHost.InitializeDatabaseAsync(host);
await host.RunAsync();
