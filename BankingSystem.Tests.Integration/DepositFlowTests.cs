using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.UseCases.Accounts.DepositBankAccount;
using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

public class DepositFlowTests : IClassFixture<InfrastructureTestFixture>
{
    private readonly ServiceProvider _services;

    public DepositFlowTests(InfrastructureTestFixture fixture)
    {
        _services = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Deposit_Should_Update_Balance_And_Create_Transaction()
    {
        // Arrange
        var db = _services.GetRequiredService<ApplicationDbContext>();
        var uow = _services.GetRequiredService<IUnitOfWork>();
        var customerRepo = _services.GetRequiredService<ICustomerRepository>();
        var ibanGen = _services.GetRequiredService<IIbanGenerator>();

        var customer = new Customer(
            "jane123",
            "Jane",
            "Doe",
            new PhoneNumber("+359888777666"),
            new Address("Street", "Sofia", 1000, "BG"),
            new EGN("0987654321", new DateOnly(1985, 5, 15), Gender.Female)
        );

        var account = customer.OpenAccount(AccountType.Checking, 500, ibanGen);

        await customerRepo.SaveAsync(customer);
        await uow.SaveChangesAsync();

        var handler = new DepositBankAccountHandler(
            customerRepo,
            new DepositBankAccountValidator(),
            uow
        );

        var command = new DepositBankAccountCommand(
            customer.Id,
            account.Id,
            200
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);

        var updated = await customerRepo.GetByIdAsync(customer.Id);
        var updatedAccount = updated.GetAccountById(account.Id);

        Assert.Equal(700, updatedAccount.Balance);  // 500 + 200

        var transactions = db.Transactions.ToList();
        Assert.Single(transactions);
        Assert.Equal(0, transactions.First().TransactionEntries.Sum(x => x.Amount));
    }
}