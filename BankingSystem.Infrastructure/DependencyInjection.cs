

namespace BankingSystem.Infrastructure
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Application.DomainEventHandlers;
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.DomainService;
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


            //domain services
            services.AddScoped<IAccountFactory, AccountFactory>();

            //services - Use FakeIbanGenerator for now (has proper checksum validation)
            // TODO: Implement real IbanGenerator with bank integration, then use environment-based registration
            services.AddScoped<IIbanGenerator, FakeIbanGenerator>();

            //event dispatcher
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            //domain event handlers
            services.Scan(scan => scan
            .FromAssembliesOf(typeof(AccountCreditedEventHandler))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            //domain service - configured with vault account ID from appsettings
            services.AddScoped<ITransactionDomainService, TransactionDomainService>(sp =>
            {
                var vaultAccountId = configuration["BankInternalAccounts:VaultAccountId"];
                if (string.IsNullOrEmpty(vaultAccountId) || !Guid.TryParse(vaultAccountId, out var vaultId))
                {
                    throw new InvalidOperationException("BankInternalAccounts:VaultAccountId must be configured in appsettings.json");
                }
                return new TransactionDomainService(vaultId);
            });

            return services;
        }
    }
}
