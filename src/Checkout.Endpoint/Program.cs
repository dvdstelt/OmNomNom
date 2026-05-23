using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Services.AddCheckoutEndpoint();

var host = hostBuilder.Build();
Console.Title = host.Services.GetRequiredService<IHostEnvironment>().ApplicationName;
await host.RunAsync();
