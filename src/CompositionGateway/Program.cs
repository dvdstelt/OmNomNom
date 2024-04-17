using ServiceComposer.AspNetCore;
using Temporary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(
            policy =>
            {
                policy.WithOrigins("https://127.0.0.1:5173", "https://localhost:5173")
                       .AllowAnyHeader();
            }));
builder.Services.AddControllers();
builder.Services.AddViewModelComposition(o =>
{
    o.EnableWriteSupport();
});

// Add cache which is used for storing the cart for now.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
});
builder.Services.AddSingleton<OrderStorage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseSession();

app.UseHttpsRedirection();
app.MapCompositionHandlers();

app.Run();