using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using ServiceComposer.AspNetCore;
using WorkflowComposer;
using WorkflowComposer.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
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
    // Required so composers can call request.SetActionResult(...) to
    // short-circuit a composed response (e.g. returning 410 Gone when
    // a cart slice has been reaped). ServiceComposer 5.2 refuses to
    // honour an action result unless output formatters are wired in.
    options.ResponseSerialization.UseOutputFormatters = true;
});
builder.Services.AddControllers()
    .AddJsonOptions(json =>
    {
        // Output formatters serialize ExpandoObject keys verbatim;
        // composers assign PascalCase (vm.Products = ...) but the
        // SvelteKit frontend reads camelCase. Make the formatter
        // lowercase the first letter so the wire shape stays the
        // same as ServiceComposer's own writer used to produce.
        json.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        json.JsonSerializerOptions.DictionaryKeyPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddHealthChecks();

var endpointConfiguration = new EndpointConfiguration("CompositionGateway");
endpointConfiguration.Configure();
endpointConfiguration.SendOnly();

// WorkflowComposer + SQLite store: workflow state for in-flight
// checkouts lives in checkout.db. Per-page slice writes use plain
// SqliteConnection; the workflow submit (added once all four service
// boundaries have migrated) will use TransactionalSession against
// this same file so the dispatch fan-out is atomic with the
// "submitted" flag.
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseWorkflowComposer();

app.UseCors();

app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.MapCompositionHandlers();

Console.Title = "Composition Gateway";

app.Run();