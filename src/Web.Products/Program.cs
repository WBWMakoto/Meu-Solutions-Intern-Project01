using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductsAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // This adds MVC support, including TempData.
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Products API",
        Version = "v1",
        Description = "API for managing products. <Note for mentor, page endpoint at Home/Index"
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader();
        });
});

// Add DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API V1");
        c.RoutePrefix = string.Empty; // Makes Swagger UI available at the root of the app
    });
}

// Enable HTTPS redirection
app.UseHttpsRedirection();
app.UseStaticFiles();
// Enable CORS for all origins
app.UseCors("AllowAll");

// Enable Authorization Middleware
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/Home/Index"); // Điều hướng đến Index.cshtml thông qua controller
    }
    else
    {
        await next(); // Tiếp tục cho các middleware khác
    }
});

// Map Controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the app
app.Run();
