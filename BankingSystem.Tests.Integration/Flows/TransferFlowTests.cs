using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.UseCases.TransferBankAccount;
using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

public class TransferFlowTests : IClassFixture<InfrastructureTestFixture>
{
    private readonly ServiceProvider _services;

    public TransferFlowTests(InfrastructureTestFixture fixture)
    {
        _services = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Transfer_Between_Different_Customers_Should_Update_Balances_And_Create_Transaction()
    {
        // Arrange
        var db = _services.GetRequiredService<ApplicationDbContext>();
        var uow = _services.GetRequiredService<IUnitOfWork>();
        var customerRepo = _services.GetRequiredService<ICustomerRepository>();
        var ibanGen = _services.GetRequiredService<IIbanGenerator>();
        var factory = _services.GetRequiredService<IAccountFactory>();
        var transferDomainService = _services.GetRequiredService<ITransferDomainService>();

        // Create SENDER customer with one account (1000 balance)
        var sender = new Customer(
            "john_sender",
            "John",
            "Sender",
            new PhoneNumber("+359888111111"),
            new Address("Sender St", "Sofia", 1000, "BG"),
            new EGN("5001010001", new DateOnly(1950, 1, 1), Gender.Male)
        );
        var senderAccount = sender.OpenAccount(AccountType.Checking, 1000, ibanGen, factory);
        await customerRepo.SaveAsync(sender);
        await uow.SaveChangesAsync();

        // Create RECEIVER customer with one account (0 balance)
        var receiver = new Customer(
            "jane_receiver",
            "Jane",
            "Receiver",
            new PhoneNumber("+359888222222"),
            new Address("Receiver St", "Plovdiv", 2000, "BG"),
            new EGN("6002020002", new DateOnly(1960, 2, 2), Gender.Female)
        );
        var receiverAccount = receiver.OpenAccount(AccountType.Checking, 0, ibanGen, factory);
        await customerRepo.SaveAsync(receiver);
        await uow.SaveChangesAsync();

        // Create handler with domain service
        var handler = new TransferBankAccountHandler(
            customerRepo,
            new TransferBankAccountValidator(),
            uow,
            transferDomainService
        );

        // Create command with BOTH customer IDs
        var command = new TransferBankAccountCommand(
            sender.Id,
            receiver.Id,
            senderAccount.Id,
            receiverAccount.Id,
            300
        );

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Reload BOTH customers to verify changes
        var updatedSender = await customerRepo.GetByIdAsync(sender.Id);
        var updatedReceiver = await customerRepo.GetByIdAsync(receiver.Id);

        var updatedSenderAccount = updatedSender.GetAccountById(senderAccount.Id);
        var updatedReceiverAccount = updatedReceiver.GetAccountById(receiverAccount.Id);

        // Verify balances updated correctly
        Assert.Equal(700, updatedSenderAccount.Balance);      // 1000 - 300
        Assert.Equal(300, updatedReceiverAccount.Balance);    // 0 + 300

        // Validate Transaction created with double-entry bookkeeping
        var transactions = db.Transactions.ToList();
        Assert.Single(transactions);

        var transaction = transactions.First();
        Assert.Equal(0, transaction.TransactionEntries.Sum(x => x.Amount)); // balanced
    }
}
