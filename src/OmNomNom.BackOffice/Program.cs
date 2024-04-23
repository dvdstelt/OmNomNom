using ServiceComposer.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting();
builder.Services.AddControllersWithViews();
builder.Services.AddViewModelComposition(options =>
{
    options.EnableCompositionOverControllers();
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