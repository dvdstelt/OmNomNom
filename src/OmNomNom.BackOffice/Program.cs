using System.Reflection;
using ITOps.Shared.EndpointConfiguration;
using ITOps.Shared.Integration;
using ServiceComposer.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting();
builder.Services.AddControllersWithViews();
builder.Services.AddViewModelComposition(options =>
{
    options.EnableCompositionOverControllers();
});

// Typed HttpClient for talking to the CompositionGateway. In development
// we skip chain validation because vite.config.js exports the ASP.NET
// dev cert without running `dotnet dev-certs https --trust`, so .NET's
// HttpClient (unlike browsers) refuses the cert with UntrustedRoot.
// Both endpoints are on localhost, so this is acceptable for dev only.
builder.Services.AddHttpClient("composition-gateway", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7126");
    })
    .ConfigurePrimaryHttpMessageHandler(provider =>
    {
        var handler = new HttpClientHandler();
        var env = provider.GetRequiredService<IHostEnvironment>();
        if (env.IsDevelopment())
        {
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }
        return handler;
    });

DataProviders.RegisterAll(builder.Services);

builder.Host.UseNServiceBus(c =>
{
    var endpointConfiguration = new EndpointConfiguration("OmNomNomBackOffice");
    endpointConfiguration.Configure();

    return endpointConfiguration;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.MapControllers();
app.MapCompositionHandlers();

Console.Title = "BackOffice Server";

app.Run();