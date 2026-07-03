using System.Text.Json;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using ServiceComposer.AspNetCore;
using WorkflowComposer;
using WorkflowComposer.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddViewModelComposition(options =>
{
    options.ResponseSerialization.UseOutputFormatters = true;
});
builder.Services.AddControllers()
    .AddJsonOptions(json =>
    {
        json.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        json.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

var endpointConfiguration = new EndpointConfiguration("CompositionGateway");
endpointConfiguration.Configure();
endpointConfiguration.SendOnly();

builder.Services.AddWorkflowComposer(workflow =>
{
    workflow.UseSqliteStore(endpointConfiguration, new SqliteWorkflowStoreOptions
    {
        ConnectionString = SqliteStorage.GetConnectionString("checkout"),
        ProcessorEndpoint = "Checkout"
    });

    workflow.DiscoverSlices();
});

builder.Services.AddNServiceBusEndpoint(endpointConfiguration);

var app = builder.Build();

app.UseWorkflowComposer();

// The frontend reaches the gateway same-origin via a reverse proxy
// (Vite in dev, nginx in the container), so no CORS is needed. In the
// container the gateway listens on plain HTTP behind a TLS-terminating
// proxy, so HTTPS redirection only applies to local dev.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapCompositionHandlers();

Console.Title = "Composition Gateway";

app.Run();
