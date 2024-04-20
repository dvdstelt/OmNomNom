using Catalog.Data;
using Catalog.Data.Migrations;
using ITOps.Shared;
using ITOps.Shared.EndpointConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string EndpointName = "Catalog";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

// Configure LiteDb
hostBuilder.Services.AddSingleton<CatalogDbContext>(provider =>
{
    var dbOptions = new LiteDbOptions("catalog", DatabaseInitializer.Initialize);
    return new CatalogDbContext(dbOptions);
});

// Configure NServiceBus
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure();
hostBuilder.UseNServiceBus(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

await host.RunAsync();