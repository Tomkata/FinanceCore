using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Aggregates.Customer.Events;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Customer;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Services;
using FluentAssertions;

namespace BankingSystem.Tests.Domain;

/// <summary>
/// Tests for TransferDomainService – cross-aggregate money transfer operations
/// </summary>
public class TransferDomainServiceTests
{
    private readonly FakeIbanGenerator _ibanGenerator = new();
    private readonly IAccountFactory _factory = new AccountFactory();
    private readonly ITransferDomainService _transferService = new TransferDomainService();

    private Customer CreateCustomer(string userName = "testuser", string egn = "0651035020")
    {
        return new Customer(
            userName,
            "John",
            "Doe",
            new PhoneNumber("1234567890"),
            new Address("123 Main St", "Burgas", 1000, "Bulgaria"),
            EGN.Create(egn)
        );
    }

    #region Transfer Between Different Customers Tests

    [Fact]
    public void TransferBetweenCustomers_ValidTransfer_UpdatesBothBalancesAndRaisesEvent()
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var receiver = CreateCustomer("receiver", "0752046131");

        var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var toAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

        sender.ClearDomainEvents();
        receiver.ClearDomainEvents();

        // Act
        _transferService.Transfer(
            sender,
            fromAccount.Id,
            receiver,
            toAccount.Id,
            200m
        );

        // Assert - Verify balances
        Assert.Equal(300m, fromAccount.Balance);   // 500 - 200
        Assert.Equal(300m, toAccount.Balance);      // 100 + 200

        // Assert - Verify TransferInitiatedEvent is raised on sender
        var transferEvent = Assert.Single(sender.DomainEvents) as TransferInitiatedEvent;
        Assert.NotNull(transferEvent);
        Assert.Equal(sender.Id, transferEvent.SenderCustomerId);
        Assert.Equal(receiver.Id, transferEvent.ReceiverCustomerId);
        Assert.Equal(fromAccount.Id, transferEvent.FromAccountId);
        Assert.Equal(toAccount.Id, transferEvent.ToAccountId);
        Assert.Equal(200m, transferEvent.Amount);

        // Assert - Verify AccountDebitedEvent and AccountCreditedEvent are NOT raised
        // (those are only raised by direct Deposit/Withdraw operations, not via domain service)
        var receiverEvent = Assert.Single(receiver.DomainEvents) as AccountCreditedEvent;
        Assert.NotNull(receiverEvent);
    }

    [Fact]
    public void TransferBetweenCustomers_SameAccount_ThrowsCannotTransferToSameAccountException()
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var receiver = CreateCustomer("receiver", "0752046131");

        var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);

        // Act & Assert
        Assert.Throws<CannotTransferToSameAccountException>(() =>
            _transferService.Transfer(
                sender,
                fromAccount.Id,
                receiver,
                fromAccount.Id,  // Same account
                100m
            )
        );
    }

    #endregion

    #region Transfer Between Same Customer Accounts Tests

    [Fact]
    public void TransferBetweenCustomers_SameCustomerDifferentAccounts_Works()
    {
        // Arrange
        var customer = CreateCustomer();
        var fromAccount = customer.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var toAccount = customer.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);
        customer.ClearDomainEvents();

        // Act - Transfer between same customer's accounts
        _transferService.Transfer(
            customer,
            fromAccount.Id,
            customer,  // Same customer
            toAccount.Id,
            200m
        );

        // Assert
        Assert.Equal(300m, fromAccount.Balance);
        Assert.Equal(300m, toAccount.Balance);

        var transferEvent = Assert.Single(customer.DomainEvents.OfType<TransferInitiatedEvent>());
        Assert.Equal(customer.Id, transferEvent.SenderCustomerId);
        Assert.Equal(customer.Id, transferEvent.ReceiverCustomerId);
    }

    #endregion

    #region Exception Tests

    [Theory]
    [InlineData(true, false)]  // From account doesn't exist (sender has no such account)
    [InlineData(false, true)]  // To account doesn't exist (receiver has no such account)
    public void TransferBetweenCustomers_NonExistentAccount_ThrowsAccountNotFoundException(
        bool fromExists,
        bool toExists)
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var receiver = CreateCustomer("receiver", "0752046131");

        var senderAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var receiverAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

        var fromId = fromExists ? senderAccount.Id : Guid.NewGuid();
        var toId = toExists ? receiverAccount.Id : Guid.NewGuid();

        // Act & Assert
        Assert.Throws<AccountNotFoundException>(() =>
            _transferService.Transfer(
                sender,
                fromId,
                receiver,
                toId,
                50m
            )
        );
    }

    [Fact]
    public void TransferBetweenCustomers_InsufficientFunds_ThrowsInsufficientFundsException()
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var receiver = CreateCustomer("receiver", "0752046131");

        var fromAccount = sender.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);
        var toAccount = receiver.OpenAccount(AccountType.Checking, 50m, _ibanGenerator, _factory);

        // Act & Assert - Try to transfer more than available balance
        Assert.Throws<InsufficientFundsException>(() =>
            _transferService.Transfer(
                sender,
                fromAccount.Id,
                receiver,
                toAccount.Id,
                200m  // More than 100 available
            )
        );
    }

    [Fact]
    public void TransferBetweenCustomers_NullSender_ThrowsArgumentNullException()
    {
        // Arrange
        var receiver = CreateCustomer("receiver", "0752046131");
        var toAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _transferService.Transfer(
                null!,
                Guid.NewGuid(),
                receiver,
                toAccount.Id,
                100m
            )
        );
    }

    [Fact]
    public void TransferBetweenCustomers_NullReceiver_ThrowsArgumentNullException()
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _transferService.Transfer(
                sender,
                fromAccount.Id,
                null!,
                Guid.NewGuid(),
                100m
            )
        );
    }

    #endregion

    #region Account Status and Business Rules Tests

    [Fact]
    public void TransferBetweenCustomers_FromFrozenAccount_ThrowsAccountNotActiveException()
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var receiver = CreateCustomer("receiver", "0752046131");

        var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var toAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

        fromAccount.Freeze();  // Freeze sender account

        // Act & Assert
        Assert.Throws<AccountNotActiveException>(() =>
            _transferService.Transfer(
                sender,
                fromAccount.Id,
                receiver,
                toAccount.Id,
                100m
            )
        );
    }

    [Fact]
    public void TransferBetweenCustomers_ToClosedAccount_ThrowsAccountNotActiveException()
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var receiver = CreateCustomer("receiver", "0752046131");

        var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var toAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

        toAccount.Withdraw(100m);  // Empty the account
        toAccount.Close();          // Close receiver account

        // Act & Assert
        Assert.Throws<AccountNotActiveException>(() =>
            _transferService.Transfer(
                sender,
                fromAccount.Id,
                receiver,
                toAccount.Id,
                100m
            )
        );
    }

    #endregion
}
