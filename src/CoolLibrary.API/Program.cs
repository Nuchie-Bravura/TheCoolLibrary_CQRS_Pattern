using CoolLibrary.API;
using CoolLibrary.API.Extensions;
using CoolLibrary.Application.Extensions;

using CoolLibrary.Infrastructure.Data;
using CoolLibrary.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);

// =====================
// 1. API Layer  {Controllers + JSON, CORS, Swagger,Heatlz checks, API Versioning}
// =====================
builder.Services.AddApiServices();

// =====================
// 2. Infrastructure Layer {EFCore + Identity + Repositories Types} blob storage connection string
// =====================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured.");
builder.Services.AddInfrastructureServices(connectionString, builder.Configuration);

// =====================
// 3. Application Layer {AutoMapper , Service complex use cases [LoanRequest , Token, Create/Delete Authors or Books] }
// =====================
builder.Services.AddApplicationServices();



// =====================
// 5. Azure Key Vault (opcional)
// =====================

builder.Configuration.AddAzureKeyVaultIfConfigured(builder.Environment);


// =====================
// 6. JWT Authentication
// =====================

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// =====================
// 7. Seed Database
// =====================
// Skip seeding in Testing environment (used by integration tests)
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            logger.LogInformation("Seeding database...");
            await DatabaseSeeder.SeedAsync(services);
            logger.LogInformation("Database seeding completed!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding database");
            throw;
        }
    }
}

// =====================
// 8. Middleware
// =====================

// Archivos estáticos (HTML de bienvenida)
app.UseDefaultFiles(); // Esto permite que index.html se sirva en la raíz
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var versionProvider = app.Services.GetRequiredService<Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider>();
        foreach (var description in versionProvider.ApiVersionDescriptions.Reverse())
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"CoolLibrary API {description.GroupName}");
        }
        c.RoutePrefix = "swagger"; // Swagger ahora está en /swagger, no en la raíz
        c.DocumentTitle = "CoolLibrary API - Swagger";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.DisplayRequestDuration();
        c.EnableFilter();
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();



// Health checks + Controllers
app.MapHealthChecks("/healthz");
app.MapControllers();

app.Run();

public partial class Program { } // For integration testing purposes
