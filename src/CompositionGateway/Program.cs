using Catalog.ServiceComposition.Workflow;
using Finance.ServiceComposition.Workflow;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Sqlite;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Workflow;
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
builder.Services.AddViewModelComposition();
builder.Services.AddControllers();

var endpointConfiguration = new EndpointConfiguration("CompositionGateway");
endpointConfiguration.Configure(configureRouting: s =>
{
    // TODO: Figure out how this can be defined per service and not globally
    s.RouteToEndpoint(typeof(Finance.Endpoint.Messages.Commands.SubmitOrderItems).Assembly, "Finance");
    s.RouteToEndpoint(typeof(Catalog.Endpoint.Messages.Commands.SubmitOrderItems).Assembly, "Catalog");
    s.RouteToEndpoint(typeof(Shipping.Endpoint.Messages.Commands.SubmitDeliveryOption).Assembly, "Shipping");
    s.RouteToEndpoint(typeof(PaymentInfo.Endpoint.Messages.Commands.SubmitPaymentInfo).Assembly, "PaymentInfo");
});
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

    workflow.RegisterSlicesFromAssembliesOf(
        typeof(CartWorkflowSlice),
        typeof(BillingAddressWorkflowSlice),
        typeof(ShippingAddressWorkflowSlice));
});

builder.UseNServiceBus(endpointConfiguration);

// Distributed cache still serves PaymentInfo cart state until that
// service boundary migrates to workflow slices.
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.UseHttpsRedirection();
app.MapCompositionHandlers();

Console.Title = "Composition Gateway";

app.Run();