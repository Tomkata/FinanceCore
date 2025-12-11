

namespace BankingSystem.Infrastructure
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Application.DomainEventHandlers;
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.DomainServices;
    using BankingSystem.Infrastructure.Data;
    using BankingSystem.Infrastructure.DomainEvents;
    using BankingSystem.Infrastructure.Persistence;
    using BankingSystem.Infrastructure.Repositories;
    using BankingSystem.Infrastructure.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            //db context
            services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //repostories
            services.Scan(scan => scan
        .FromAssemblyOf<AccountRepository>()
        .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
        .AsImplementedInterfaces()
        .WithScopedLifetime());
            

            //services
            services.AddScoped<IIbanGenerator, IbanGenerator>();
            services.AddScoped<IIbanGenerator, FakeIbanGenerator>();

            //event dispatcher
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            //domain event handlers
            services.Scan(scan => scan
            .FromAssembliesOf(typeof(AccountCreditedEventHandler))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            //domain service
            services.AddScoped<ITransactionDomainService, TransactionDomainService>();

            return services;
        }
    }
}
