

namespace BankingSystem.Application
{
    using BankingSystem.Application.UseCases.Customers.CreateCustomer;
    using Microsoft.Extensions.DependencyInjection;
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //Validators
            services.AddScoped<CreateCustomerValidator>();

            //Handlers
            services.AddScoped<CreateCustomerHandler>();
                
            return services;
        }
    }
}
