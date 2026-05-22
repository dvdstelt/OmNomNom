using Finance.Data;
using Finance.Data.Seed;
using Finance.Endpoint.Handlers;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string EndpointName = "Finance";

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

var sqliteConnectionString = SqliteStorage.GetConnectionString("finance");

hostBuilder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlite(sqliteConnectionString));

// Configure NServiceBus. Handlers are registered explicitly via the
// source-generated AddHandler<T>() path (NServiceBus 10.2); reflection
// scanning still finds them too and the registrations dedupe.
var endpointConfiguration = new EndpointConfiguration(EndpointName);
endpointConfiguration.Configure(sqliteConnectionString);
endpointConfiguration.AddHandler<OrderCancelledHandler>();
endpointConfiguration.AddHandler<OrderPlacedHandler>();
endpointConfiguration.AddHandler<SubmitBillingAddressHandler>();
endpointConfiguration.AddHandler<SubmitDeliveryOptionHandler>();
endpointConfiguration.AddHandler<SubmitOrderItemsHandler>();
hostBuilder.Services.AddNServiceBusEndpoint(endpointConfiguration);

var host = hostBuilder.Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
Console.Title = hostEnvironment.ApplicationName;

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext);
}

await host.RunAsync();
