using Finance.Data;
using Finance.Data.Migrations;
using ITOps.Shared;
using ITOps.Shared.EndpointConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string EndpointName = "Finance";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

// Configure LiteDb
hostBuilder.Services.AddSingleton<FinanceDbContext>(provider =>
{
    var dbOptions = new LiteDbOptions("finance", DatabaseInitializer.Initialize);
    return new FinanceDbContext(dbOptions);
});

// Configure NServiceBus
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure();
hostBuilder.UseNServiceBus(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

await host.RunAsync();