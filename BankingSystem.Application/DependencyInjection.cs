

namespace BankingSystem.Application
{
    using BankingSystem.Application.UseCases.Customers.CreateCustomer;
    using Mapster;
    using MapsterMapper;
    using Microsoft.Extensions.DependencyInjection;
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //Validators
            services.AddScoped<CreateCustomerValidator>();

            //Handlers
            services.AddScoped<CreateCustomerHandler>();

            //mapper
            services.AddSingleton<IMapper>(new Mapper(TypeAdapterConfig.GlobalSettings));
                
            return services;
        }
    }
}
