using CoolLibrary.Application.Services.Cache;
using CoolLibrary.Infrastructure.Data;
using CoolLibrary.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoolLibraryIntegrationTests;

public class CoolLibraryWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Add test configuration
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "InMemory",
                ["ConnectionStrings:Redis"] = "disabled",
                ["Jwt:Key"] = "ThisIsATestSecretKeyForIntegrationTestingPurposesOnly123456789012345678901234567890",
                ["Jwt:Issuer"] = "https://test-api.com",
                ["Jwt:Audience"] = "https://test-api.com",
                ["AzureStorage:ConnectionString"] = "UseDevelopmentStorage=true",
                ["AzureStorage:ContainerName"] = "test-container",
                ["KeyVault:Url"] = ""
            }!);
        });

        builder.ConfigureServices((context, services) =>
        {
            // Remove existing DbContext registration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>));
            
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Remove DbContext registrations
            var dbContextRegistrations = services
                .Where(d => d.ServiceType == typeof(LibraryDbContext))
                .ToList();
            
            foreach (var registration in dbContextRegistrations)
            {
                services.Remove(registration);
            }

            // Add InMemory DbContext with unique database name per test run
            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
            
            // Remove Redis Cache
            var redisCacheDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache) 
                && d.ImplementationType?.FullName?.Contains("Redis") == true);
            
            if (redisCacheDescriptor != null)
            {
                services.Remove(redisCacheDescriptor);
            }
            
            // Remove RedisCacheService
            var redisCacheServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICacheService) 
                && d.ImplementationType == typeof(RedisCacheService));
            
            if (redisCacheServiceDescriptor != null)
            {
                services.Remove(redisCacheServiceDescriptor);
            }
            
            // Add InMemory Cache
            services.AddDistributedMemoryCache();
            services.AddScoped<ICacheService, InMemoryCacheService>();

            // Bypass authorization for integration tests
            services.AddSingleton<IAuthorizationHandler, AllowAnonymousAuthorizationHandler>();
        });
        
        builder.UseEnvironment("Testing");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Initialize database after host is created
        using (var scope = host.Services.CreateScope())
        {
            var database = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            database.Database.EnsureCreated();
        }

        return host;
    }
}

// Authorization handler that allows all requests
public class AllowAnonymousAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var requirement in context.PendingRequirements.ToList())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
