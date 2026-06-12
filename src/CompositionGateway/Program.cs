using System.Text.Json;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using ServiceComposer.AspNetCore;
using WorkflowComposer;
using WorkflowComposer.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(
            policy =>
            {
                policy.WithOrigins("https://127.0.0.1:5173", "https://localhost:5173")
                       .AllowAnyHeader();
            }));
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

app.UseCors();

app.UseHttpsRedirection();
app.MapCompositionHandlers();

Console.Title = "Composition Gateway";

app.Run();
