using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Enums;
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

        var tempProvider = services.BuildServiceProvider();
        var db = tempProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        var ibanGen = new FakeIbanGenerator();

        var bankVaultCustomer = new Customer(
            "BANK_VAULT",
            "Bank",
            "Vault",
            new PhoneNumber("+359000000000"),
            new Address("Bank HQ", "Sofia", 1000, "BG"),
            new EGN("0000000000", new DateOnly(2000, 1, 1), Gender.Male)
        );

        var bankVaultAccount = bankVaultCustomer.OpenAccount(
            AccountType.Checking,
            1_000_000_000, 
            ibanGen
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