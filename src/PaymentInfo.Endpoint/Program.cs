using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentInfo.Data;
using PaymentInfo.Data.Seed;

const string EndpointName = "PaymentInfo";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddDbContext<PaymentInfoDbContext>(options =>
    options.UseSqlite(SqliteStorage.GetConnectionString("paymentinfo")));

// Configure NServiceBus
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure();
hostBuilder.UseNServiceBus(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentInfoDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext);
}

await host.RunAsync();
