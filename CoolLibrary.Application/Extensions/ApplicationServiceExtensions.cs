using CoolLibrary.Application.Mappings;


using CoolLibrary.Application.Services.LoansAndReservations;
using CoolLibrary.Application.Services.Token;
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
  

            //services.AddScoped<LoanApprovalService>();
            //services.AddScoped<ReservationService>();
            //services.AddScoped<ReturnLoanService>();
            //services.AddScoped<GetUserLoansService>();


            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }
    }
}