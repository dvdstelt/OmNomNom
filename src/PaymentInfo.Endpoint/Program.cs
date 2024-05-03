using ITOps.Shared;
using ITOps.Shared.EndpointConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentInfo.Data;
using PaymentInfo.Data.Migrations;

const string EndpointName = "PaymentInfo";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

// Configure LiteDb
hostBuilder.Services.AddSingleton<PaymentInfoDbContext>(provider =>
{
    var dbOptions = new LiteDbOptions("paymentinfo", DatabaseInitializer.Initialize);
    return new PaymentInfoDbContext(dbOptions);
});

// Configure NServiceBus
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure();
hostBuilder.UseNServiceBus(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

await host.RunAsync();