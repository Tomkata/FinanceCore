using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Data;
using BankingSystem.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class InfrastructureTestFixture : IDisposable
{
    public ServiceProvider ServiceProvider { get; }
    public Guid BankVaultAccountId { get; }

    private readonly SqliteConnection _connection;

    public InfrastructureTestFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlite(_connection));

        services.AddScoped<IAccountFactory, AccountFactory>();
        services.AddScoped<IIbanGenerator, FakeIbanGenerator>();

        var tempProvider = services.BuildServiceProvider();
        var db = tempProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        var ibanGen = tempProvider.GetRequiredService<IIbanGenerator>();
        var factory = tempProvider.GetRequiredService<IAccountFactory>();

        var bankVaultCustomer = new Customer(
            "BANK_VAULT",
            "Bank",
            "Vault",
            new PhoneNumber("+359000000000"),
            new Address("Bank HQ", "Sofia", 1000, "BG"),
            EGN.Create("5001010001")  // Valid EGN: born Jan 1, 1950
        );

        var bankVaultAccount = bankVaultCustomer.OpenAccount(
            AccountType.Checking,
            1_000_000_000,
            ibanGen,
            factory
        );

        BankVaultAccountId = bankVaultAccount.Id;

        db.Customers.Add(bankVaultCustomer);
        db.SaveChanges();

        services.AddInfrastructureForTests(BankVaultAccountId);

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _connection?.Dispose();
        ServiceProvider?.Dispose();
    }
}
