using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Infrastructure.Data;
using CoolLibrary.Infrastructure.Repositories;
using CoolLibrary.Infrastructure.Services;
using CoolLibrary.Application.Services.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoolLibrary.Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            string connectionString, 
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Identity
            services.AddIdentityCore<ApplicationUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<LibraryDbContext>();

            // Redis Cache (con fallback a InMemory para desarrollo)
            var redisConnection = configuration.GetConnectionString("Redis");
            
            if (!string.IsNullOrEmpty(redisConnection) && redisConnection != "disabled")
            {
                // Remote Redis DB
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                    options.InstanceName = "CoolLibrary_";
                });
                services.AddScoped<ICacheService, RedisCacheService>();
            }
            else
            {
                // Fallback CPU RAM
                services.AddMemoryCache();
                services.AddScoped<ICacheService, InMemoryCacheService>();
            }

            // Repositories
            services.AddScoped<IAuthors, AuthorsRepository>();
            services.AddScoped<IBooks, BooksRepository>();
            services.AddScoped<ICustomers, CustomersRepository>();
            services.AddScoped<ILoans, LoansRepository>();
            services.AddScoped<IArchiveStorage, AzureArchiveStorageRepository>();

            return services;
        }
    }
}