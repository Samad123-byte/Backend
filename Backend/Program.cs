using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Data.Repositories;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization to use PascalCase (matching your backend models)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Keep PascalCase naming (ProductId, Name, etc.)
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // This keeps original property names
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;

        // Optional: Make JSON more readable
        options.JsonSerializerOptions.WriteIndented = true;

        // Handle null values gracefully
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories for Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISaleDetailRepository, SaleDetailRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ISalespersonRepository, SalespersonRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("ReactApp");

app.UseAuthorization();
app.MapControllers();

app.Run();