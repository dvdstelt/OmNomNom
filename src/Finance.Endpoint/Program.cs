using Finance.Data;
using Finance.Data.Seed;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string EndpointName = "Finance";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlite(SqliteStorage.GetConnectionString("finance")));

// Configure NServiceBus
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure();
hostBuilder.UseNServiceBus(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext);
}

await host.RunAsync();
