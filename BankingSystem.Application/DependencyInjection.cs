

namespace BankingSystem.Application
{
    using BankingSystem.Application.UseCases.Customers.CreateCustomer;
    using Mapster;
    using MapsterMapper;
    using Microsoft.Extensions.DependencyInjection;
    using Scrutor;
    

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.Scan(scan => scan
            .FromAssemblyOf<CreateCustomerHandler>()
            .AddClasses(classes => classes.Where(x => x.Name.EndsWith("Handler")))
            .AsSelf()
            .WithScopedLifetime());

            services.Scan(scan => scan
            .FromAssemblyOf<CreateCustomerValidator>()
            .AddClasses(classes => classes.Where(x => x.Name.EndsWith("Validator")))
            .AsSelf()
            .WithScopedLifetime());
            

            //mapper
                        services.AddSingleton<IMapper>(new Mapper(TypeAdapterConfig.GlobalSettings));
                
            return services;
        }
    }
}
