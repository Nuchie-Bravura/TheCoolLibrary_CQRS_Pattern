using CoolLibrary.Application.Mappings;
using CoolLibrary.Application.Services;
using Microsoft.Extensions.DependencyInjection;



namespace CoolLibrary.Application.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Application Services
            services.AddScoped<LoanRequestService>();
            services.AddScoped<TokenService>();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }
    }
}