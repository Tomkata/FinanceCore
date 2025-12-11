using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.DomainEventHandlers;
using BankingSystem.Domain.Common;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Infrastructure.DomainEvents;
using BankingSystem.Infrastructure.Persistence;
using BankingSystem.Infrastructure.Repositories;
using BankingSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureTestExtensions
{
    public static IServiceCollection AddInfrastructureForTests(
        this IServiceCollection services,
        Guid bankVaultAccountId)  // Приеми като параметър
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.Scan(scan => scan
            .FromAssemblyOf<TransferInitiatedEventHandler>()
            .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        services.AddScoped<IIbanGenerator, FakeIbanGenerator>();

        // Използвай ИСТИНСКИЯ bankVaultAccountId
        services.AddScoped<ITransactionDomainService>(sp =>
            new TransactionDomainService(bankVaultAccountId));

        return services;
    }
}