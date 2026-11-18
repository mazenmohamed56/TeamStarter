using Microsoft.EntityFrameworkCore;
using TeamStarter.Api.Common;
using TeamStarter.Api.Data;
using TeamStarter.Api.Features.Categories;
using TeamStarter.Api.Features.Services;
using TeamStarter.Api.Features.Tasks;

var builder = WebApplication.CreateBuilder(args);

// ---------- Services ----------
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Unified error responses (RFC 7807 ProblemDetails)
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Feature services — register every new feature here
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// The Vue/Angular dev servers proxy /api, but CORS is needed when calling the API directly
builder.Services.AddCors(options =>
    options.AddPolicy("Frontends", policy => policy
        .WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod()));

// TODO (Week 2): add Authentication (ASP.NET Identity + JWT) and Authorization policies

var app = builder.Build();

// ---------- Pipeline ----------
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "TeamStarter API"));

    // Apply pending migrations automatically in development
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
        db.Database.Migrate();
}

app.UseCors("Frontends");

// TODO (Week 2): app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposed for integration tests (WebApplicationFactory<Program>)
public partial class Program;


