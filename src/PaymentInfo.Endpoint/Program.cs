using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentInfo.Endpoint;

var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Services.AddPaymentInfoEndpoint();

var host = hostBuilder.Build();
Console.Title = host.Services.GetRequiredService<IHostEnvironment>().ApplicationName;
await host.RunAsync();
