using System.Reflection;
using ITOps.Shared.EndpointConfiguration;
using OmNomNom.Website.Helpers;
using ServiceComposer.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting();
builder.Services.AddControllersWithViews();
builder.Services.AddViewModelComposition(options =>
{
    options.EnableCompositionOverControllers();
});

var assemblies = ReflectionHelper.GetAssemblies(".Data.dll");
var types = assemblies
    .SelectMany(a => a.GetTypes())
    .Where(t => typeof(IProvideCustomerData).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);


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

app.Run();