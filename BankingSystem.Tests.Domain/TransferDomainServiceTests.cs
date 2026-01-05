using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Aggregates.Customer.Events;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Services;
using FluentAssertions;


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

    #region Transfers Between Different Customers

   [Fact]
public void Transfer_ValidTransfer_UpdatesBalances()
{
    // Arrange
    var sender = CreateCustomer("sender", "0651035020");
    var receiver = CreateCustomer("receiver", "0752046131");

    var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
    var toAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

    // Act
    _transferService.Transfer(
        sender,
        fromAccount.Id,
        receiver,
        toAccount.Id,
        200m
    );

    // Assert
    fromAccount.Balance.Should().Be(300m);
    toAccount.Balance.Should().Be(300m);
}


    [Fact]
    public void Transfer_SameAccount_ThrowsCannotTransferToSameAccountException()
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var receiver = CreateCustomer("receiver", "0752046131");

        var account = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);

        // Act & Assert
        Assert.Throws<CannotTransferToSameAccountException>(() =>
            _transferService.Transfer(
                sender,
                account.Id,
                receiver,
                account.Id,
                100m
            )
        );
    }

    #endregion

    #region Transfers Between Accounts of the Same Customer

    [Fact]
    public void Transfer_SameCustomerDifferentAccounts_Succeeds()
    {
        // Arrange
        var customer = CreateCustomer();
        var fromAccount = customer.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var toAccount = customer.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

        customer.ClearDomainEvents();

        // Act
        _transferService.Transfer(
            customer,
            fromAccount.Id,
            customer,
            toAccount.Id,
            200m
        );

        // Assert
        fromAccount.Balance.Should().Be(300m);
        toAccount.Balance.Should().Be(300m);

        // No TransferInitiatedEvent assertions here — this test focuses on balances
    }

    #endregion

    #region Validation and Exception Scenarios

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Transfer_AccountNotFound_ThrowsAccountNotFoundException(
        bool senderAccountExists,
        bool receiverAccountExists)
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var receiver = CreateCustomer("receiver", "0752046131");

        var senderAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var receiverAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

        var fromAccountId = senderAccountExists ? senderAccount.Id : Guid.NewGuid();
        var toAccountId = receiverAccountExists ? receiverAccount.Id : Guid.NewGuid();

        // Act & Assert
        Assert.Throws<AccountNotFoundException>(() =>
            _transferService.Transfer(
                sender,
                fromAccountId,
                receiver,
                toAccountId,
                50m
            )
        );
    }

    [Fact]
    public void Transfer_InsufficientFunds_ThrowsInsufficientFundsException()
    {
        // Arrange
        var sender = CreateCustomer("sender", "0651035020");
        var receiver = CreateCustomer("receiver", "0752046131");

        var fromAccount = sender.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);
        var toAccount = receiver.OpenAccount(AccountType.Checking, 50m, _ibanGenerator, _factory);

        // Act & Assert
        Assert.Throws<InsufficientFundsException>(() =>
            _transferService.Transfer(
                sender,
                fromAccount.Id,
                receiver,
                toAccount.Id,
                200m
            )
        );
    }

    [Fact]
    public void Transfer_NullSender_ThrowsArgumentNullException()
    {
        var receiver = CreateCustomer();
        var toAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

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
    public void Transfer_NullReceiver_ThrowsArgumentNullException()
    {
        var sender = CreateCustomer();
        var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);

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

    #region Account State Rules

    [Fact]
    public void Transfer_FromFrozenAccount_ThrowsAccountNotActiveException()
    {
        var sender = CreateCustomer();
        var receiver = CreateCustomer();

        var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var toAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

        fromAccount.Freeze();

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
    public void Transfer_ToClosedAccount_ThrowsAccountNotActiveException()
    {
        var sender = CreateCustomer();
        var receiver = CreateCustomer();

        var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var toAccount = receiver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);

        toAccount.Withdraw(100m);
        toAccount.Close();

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