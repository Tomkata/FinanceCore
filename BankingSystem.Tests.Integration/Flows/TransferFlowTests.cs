using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.UseCases.TransferBankAccount;
using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using BankingSystem.Domain.DomainService;

public class TransferFlowTests : IClassFixture<InfrastructureTestFixture>
{
    private readonly ServiceProvider _services;

    public TransferFlowTests(InfrastructureTestFixture fixture)
    {
        _services = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Transfer_Should_Update_Balances_And_Create_Transaction()
    {
        // Arrange
        var db = _services.GetRequiredService<ApplicationDbContext>();
        var uow = _services.GetRequiredService<IUnitOfWork>();
        var customerRepo = _services.GetRequiredService<ICustomerRepository>();
        var ibanGen = _services.GetRequiredService<IIbanGenerator>();
        var factory = _services.GetRequiredService<IAccountFactory>();


        // Create customer
        var customer = new Customer(
            "john123",
            "John",
            "Doe",
            new PhoneNumber("+359888888888"),
            new Address("Addr", "City", 1000, "BG"),
            new EGN("7910024406", new DateOnly(1990, 1, 1), Gender.Male)
        );

        // Open two accounts
        var fromAcc = customer.OpenAccount(AccountType.Checking, 1000, ibanGen, factory);
        var toAcc = customer.OpenAccount(AccountType.Checking, 0, ibanGen, factory);

        await customerRepo.SaveAsync(customer);
        await uow.SaveChangesAsync();

        var handler = new TransferBankAccountHandler(
            customerRepo,
            new TransferBankAccountValidator(),
            uow
        );

        var command = new TransferBankAccountCommand(
            customer.Id,
            fromAcc.Id,
            toAcc.Id,
            300
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Reload customer
        var updated = await customerRepo.GetByIdAsync(customer.Id);

        var updatedFrom = updated.GetAccountById(fromAcc.Id);
        var updatedTo = updated.GetAccountById(toAcc.Id);

        Assert.Equal(700, updatedFrom.Balance);   // 1000 - 300
        Assert.Equal(300, updatedTo.Balance);     // 0 + 300    

        // Validate Transaction
        var transactions = db.Transactions.ToList();
        Assert.Single(transactions);

        var transaction = transactions.First();
        Assert.Equal(0, transaction.TransactionEntries.Sum(x => x.Amount)); // double-entry balanced
    }
}
