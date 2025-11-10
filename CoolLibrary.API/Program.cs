using AutoMapper;
using Azure.Identity;
using Azure;
using CoolLibrary.Application.Mappings;
using CoolLibrary.Application.Services;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;  // ← Add this for ApplicationUser
using CoolLibrary.Infrastructure.Data;
using CoolLibrary.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using CoolLibrary.API.Filters;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Configure API Versioning
// This enables versioning via URL path (e.g., /api/v1/customers, /api/v2/customers)
builder.Services.AddApiVersioning(options =>
{
    // Default version if client doesn't specify one
    options.DefaultApiVersion = new ApiVersion(1, 0);  // v1.0
    
    // Use default version when client doesn't specify
    options.AssumeDefaultVersionWhenUnspecified = true;
    
    // Report supported versions in response headers
    // Adds headers: api-supported-versions, api-deprecated-versions
    options.ReportApiVersions = true;
    
    // Read version from URL path: /api/v1/customers
    // Other options: QueryString (?api-version=1.0), Header (x-api-version)
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    // Format version as 'v{major}' in URLs (e.g., v1, v2)
    // Alternative: 'v{major}.{minor}' for v1.0, v2.0
    options.GroupNameFormat = "'v'VVV";
    
    // Replace version placeholder in route templates
    // Changes [controller] to the actual controller name
    options.SubstituteApiVersionInUrl = true;
});

var keyVaultUrl = builder.Configuration["KeyVault:Url"];
if (!string.IsNullOrWhiteSpace(keyVaultUrl) && !builder.Environment.IsDevelopment())
{
    try
    {
        builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
        Console.WriteLine($"Azure Key Vault configuration loaded from {keyVaultUrl}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[KeyVault] Could not load Key Vault: {ex.Message}");
    }
}

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Missing JWT Key. Set it in User Secrets, environment variable, or Key Vault.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Configure Database Context
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//EFCore Identity - Using ApplicationUser instead of IdentityUser
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LibraryDbContext>()
    .AddDefaultTokenProviders();



// Register Repository Services
builder.Services.AddScoped<IAuthors, AuthorsRepository>();
builder.Services.AddScoped<IBooks, BooksRepository>();
builder.Services.AddScoped<ICustomers, CustomersRepository>();
builder.Services.AddScoped<ILoans, LoansRepository>();  // ← Fixed typo

// Application services
builder.Services.AddScoped<LoanRequestService>();

// Authentication services
// TokenService: Generates JWT tokens for authenticated users
builder.Services.AddScoped<TokenService>();

// Configure AutoMapper 
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<LibraryDbContext>("database");

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to generate separate documents per API version
var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider()
    .GetRequiredService<Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider>();

builder.Services.AddSwaggerGen(options =>
{
    // Generate a Swagger document for EACH API version
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "CoolLibrary API",
            Version = description.ApiVersion.ToString(),
            Description = $"A Library Management System API built with Clean Architecture - Version {description.ApiVersion}"
                + (description.IsDeprecated ? " (DEPRECATED)" : "")
        });
    }

    // JWT Authentication in Swagger
    // This adds the "Authorize" button (🔒) in Swagger UI
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme.
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
    });

    // Enable XML Comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);

    // Add operation filter to apply security only to endpoints with [Authorize]
    options.OperationFilter<AuthorizeCheckOperationFilter>();
});

var app = builder.Build();

// SEED DATABASE WITH ROLES AND ADMIN USER
// Using Infrastructure layer's DatabaseSeeder (Clean Architecture)
Console.WriteLine("==============================================");
Console.WriteLine("🌱 INITIALIZING DATABASE SEEDING...");
Console.WriteLine("==============================================");

try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Starting database seeding process...");
        
        await DatabaseSeeder.SeedAsync(services);
        
        logger.LogInformation("Database seeding completed successfully!");
    }
    
    Console.WriteLine("==============================================");
    Console.WriteLine("✅ DATABASE SEEDING SUCCESSFUL");
    Console.WriteLine("==============================================");
}
catch (Exception ex)
{
    Console.WriteLine("==============================================");
    Console.WriteLine("❌ DATABASE SEEDING FAILED!");
    Console.WriteLine("==============================================");
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    
    // Log using the application's logger
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "❌ Fatal error during database seeding");
    
    // You can choose to either:
    // 1. Throw (stop the application from starting)
    throw;
    
    // 2. Or continue (not recommended for production)
    // Console.WriteLine("⚠️  Application will continue despite seeding failure");
}

// Get API version provider for Swagger UI
var versionDescriptionProvider = app.Services.GetRequiredService<Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Create a dropdown in Swagger UI for each API version
        foreach (var description in versionDescriptionProvider.ApiVersionDescriptions.Reverse())
        {
            c.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"CoolLibrary API {description.GroupName.ToUpperInvariant()}"
            );
        }
        
        c.RoutePrefix = string.Empty; // Swagger UI at root
        c.DocumentTitle = "CoolLibrary API Documentation";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.DisplayRequestDuration();
        c.EnableFilter();
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Health Check endpoint
app.MapHealthChecks("/healthz");

app.MapControllers();

app.Run();
