
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
builder.Services.AddInfrastructureServices(builder.Configuration.GetConnectionString("DefaultConnection"));

// =====================
// 3. Application Layer {AutoMapper , Service complex use cases [LoanRequest , Token, Create/Delete Authors or Books] }
// =====================
builder.Services.AddApplicationServices();

// =====================
// 4. Azure Key Vault (opcional)
// =====================

builder.Configuration.AddAzureKeyVaultIfConfigured(builder.Environment);


// =====================
// 5. JWT Authentication
// =====================

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// =====================
// 6. Seed Database
// =====================
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

// =====================
// 7. Middleware
// =====================
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
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "CoolLibrary API";
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
