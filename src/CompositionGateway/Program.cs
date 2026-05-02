using ITOps.Shared.EndpointConfiguration;
using ServiceComposer.AspNetCore;

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
endpointConfiguration.Configure(s =>
{
    // TODO: Figure out how this can be defined per service and not globally
    s.RouteToEndpoint(typeof(Finance.Endpoint.Messages.Commands.SubmitOrderItems).Assembly, "Finance");
    s.RouteToEndpoint(typeof(Catalog.Endpoint.Messages.Commands.SubmitOrderItems).Assembly, "Catalog");
    s.RouteToEndpoint(typeof(Shipping.Endpoint.Messages.Commands.SubmitDeliveryOption).Assembly, "Shipping");
    s.RouteToEndpoint(typeof(PaymentInfo.Endpoint.Messages.Commands.SubmitPaymentInfo).Assembly, "PaymentInfo");
});
endpointConfiguration.SendOnly();
builder.UseNServiceBus(endpointConfiguration);

// Add cache which is used for storing the cart for now.
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