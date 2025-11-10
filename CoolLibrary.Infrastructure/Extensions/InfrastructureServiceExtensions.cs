using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Infrastructure.Data;
using CoolLibrary.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CoolLibrary.Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            // DbContext
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Identity
            services.AddIdentityCore<ApplicationUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<LibraryDbContext>();

            // Repositories
            services.AddScoped<IAuthors, AuthorsRepository>();
            services.AddScoped<IBooks, BooksRepository>();
            services.AddScoped<ICustomers, CustomersRepository>();
            services.AddScoped<ILoans, LoansRepository>();

            return services;
        }
    }
}
    