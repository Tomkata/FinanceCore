using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Aggregates.Customer.Events;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Customer;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Services;
using FluentAssertions;

namespace BankingSystem.Tests.Domain;
public class CustomerTests
{
    private readonly FakeIbanGenerator _ibanGenerator = new();
    private readonly IAccountFactory _factory = new AccountFactory();
    private Customer CreateCustomer(CustomerStatus? status = null)
    {
        var customer = new Customer(
            "testuser",
            "John",
            "Doe",
            new PhoneNumber("1234567890"),
            new Address("123 Main St", "Burgas", 1000, "Bulgaria"),
            EGN.Create("0651035020")
        );

        if (status.HasValue && status != CustomerStatus.Active)
        {
            typeof(Customer).GetProperty("Status")?.SetValue(customer, status.Value);
        }

        return customer;
    }

    private (Customer customer, Account account) CreateCustomerWithAccount(
        AccountType type = AccountType.Checking,
        decimal balance = 100m,
        int? withdrawLimit = null,
        DepositTerm? depositTerm = null)
    {
        var customer = CreateCustomer();
        var account = customer.OpenAccount(
            type,
            balance,
            _ibanGenerator,
            _factory,
            withdrawLimit,
            depositTerm
        ); customer.ClearDomainEvents();
        return (customer, account);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ValidParameters_CreatesActiveCustomerWithEmptyAccounts()
    {
        var customer = CreateCustomer();

        Assert.Equal("testuser", customer.UserName);
        Assert.Equal("John", customer.FirstName);
        Assert.Equal("Doe", customer.LastName);
        Assert.NotNull(customer.PhoneNumber);
        Assert.NotNull(customer.Address);
        Assert.NotNull(customer.EGN);
        Assert.Equal(CustomerStatus.Active, customer.Status);
        Assert.Empty(customer.Accounts);
    }

    [Fact]
    public void Constructor_NullEGN_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Customer("testuser", "John", "Doe",
                new PhoneNumber("1234567890"),
                new Address("123 Main St", "Sofia", 1000, "Bulgaria"),
                null!));
    }

    #endregion

    #region OpenAccount Tests

    [Theory]
    [InlineData(AccountType.Checking, 100)]
    [InlineData(AccountType.Checking, 0)]
    public void OpenAccount_Checking_CreatesAccountSuccessfully(AccountType type, decimal balance)
    {
        var customer = CreateCustomer();

        var acc = customer.OpenAccount(AccountType.Checking, 100, _ibanGenerator, _factory);
        Assert.IsType<CheckingAccount>(acc);
    }

    [Fact]
    public void OpenAccount_Saving_WithWithdrawLimit_CreatesAccountSuccessfully()
    {
        var customer = CreateCustomer();

        var account = customer.OpenAccount(AccountType.Saving, 500m, _ibanGenerator, _factory, withdrawLimit: 3);

        var acc = customer.OpenAccount(AccountType.Saving, 100, _ibanGenerator, _factory, withdrawLimit: 3);
        var saving = Assert.IsType<SavingAccount>(acc);
        Assert.Equal(3, saving.WithdrawLimits);
    }

    [Fact]
    public void OpenAccount_Saving_WithoutWithdrawLimit_ShouldThrowWithdrawLimitRequired()
    {
        var customer = CreateCustomer();

        Action act = () => customer.OpenAccount(
            AccountType.Saving,
            100m,
            _ibanGenerator,
            _factory
        );

        act.Should().Throw<WithdrawLimitRequiredException>();
    }


    [Fact]
    public void OpenAccount_Deposit_WithDepositTerm_CreatesAccountWithMaturityDate()
    {
        var customer = CreateCustomer();
        var depositTerm = new DepositTerm(12);

        var account = customer.OpenAccount(
            AccountType.Deposit,
            1000m,
            _ibanGenerator,
            _factory,
            depositTerm: depositTerm
        );

        var deposit = Assert.IsType<DepositAccount>(account);

        Assert.NotNull(deposit.MaturityDate);
        Assert.True(deposit.MaturityDate > DateTime.UtcNow);
    }


    [Fact]
    public void OpenAccount_Deposit_WithoutDepositTerm_ShouldThrowArgumentNull()
    {
        var customer = CreateCustomer();

        Assert.Throws<ArgumentNullException>(() =>
            customer.OpenAccount(
                AccountType.Deposit,
                1000m,
                _ibanGenerator,
                _factory
            )
        );
    }


    [Fact]
    public void OpenAccount_MultipleCheckingAccounts_Succeeds()
    {
        var customer = CreateCustomer();

        customer.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);
        customer.OpenAccount(AccountType.Checking, 200m, _ibanGenerator, _factory);

        Assert.Equal(2, customer.Accounts.Count);
    }


    [Fact]
    public void OpenAccount_MultipleDepositAccounts_ThrowsCannotHaveMultipleDepositAccountsException()
    {
        var customer = CreateCustomer();
        var term = new DepositTerm(12);

        customer.OpenAccount(AccountType.Deposit, 1000m, _ibanGenerator, _factory, depositTerm: term);

        Assert.Throws<CannotHaveMultipleDepositAccountsException>(() =>
            customer.OpenAccount(AccountType.Deposit, 2000m, _ibanGenerator, _factory, depositTerm: term)
        );
    }


    [Fact]
    public void OpenAccount_InactiveCustomer_ThrowsCannotOpenAccountForInactiveCustomerException()
    {
        var customer = CreateCustomer(CustomerStatus.Inactive);

        Assert.Throws<CannotOpenAccountForInactiveCustomerException>(() =>
            customer.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory)
        );
    }


    [Fact]
    public void OpenAccount_NegativeBalance_ThrowsInvalidAmountException()
    {
        var customer = CreateCustomer();

        Assert.Throws<InvalidAmountException>(() =>
            customer.OpenAccount(AccountType.Checking, -100m, _ibanGenerator, _factory)
        );
    }


    #endregion

    #region Deposit Tests

    [Fact]
    public void Deposit_ValidAmount_IncreasesBalanceAndRaisesEvent()
    {
        var (customer, account) = CreateCustomerWithAccount(balance: 0m);

        customer.Deposit(account.Id, 50m);

        Assert.Equal(50m, account.Balance);
        var creditEvent = Assert.Single(customer.DomainEvents) as AccountCreditedEvent;
        Assert.NotNull(creditEvent);
        Assert.Equal(customer.Id, creditEvent.customerId);
        Assert.Equal(account.Id, creditEvent.accountId);
        Assert.Equal(50m, creditEvent.amount);
    }

    [Fact]
    public void Deposit_NonExistentAccount_ThrowsAccountNotFoundException()
    {
        var customer = CreateCustomer();

        Assert.Throws<AccountNotFoundException>(() =>
            customer.Deposit(Guid.NewGuid(), 100m));
    }

    #endregion

    #region Withdraw Tests

    [Fact]
    public void Withdraw_ValidAmount_DecreasesBalanceAndRaisesEvent()
    {
        var (customer, account) = CreateCustomerWithAccount(balance: 200m);

        customer.Withdraw(account.Id, 50m);

        Assert.Equal(150m, account.Balance);
        var debitEvent = Assert.Single(customer.DomainEvents) as AccountDebitedEvent;
        Assert.NotNull(debitEvent);
        Assert.Equal(customer.Id, debitEvent.customerId);
        Assert.Equal(account.Id, debitEvent.accountId);
        Assert.Equal(50m, debitEvent.amount);
    }

    [Fact]
    public void Withdraw_NonExistentAccount_ThrowsAccountNotFoundException()
    {
        var customer = CreateCustomer();

        Assert.Throws<AccountNotFoundException>(() =>
            customer.Withdraw(Guid.NewGuid(), 50m));
    }

    [Fact]
    public void Withdraw_ExceedingBalance_ThrowsInsufficientFundsException()
    {
        var (customer, account) = CreateCustomerWithAccount(balance: 100m);

        Assert.Throws<InsufficientFundsException>(() =>
            customer.Withdraw(account.Id, 200m));
    }

    #endregion

    #region Transfer Tests

    [Fact]
    public void Transfer_BetweenOwnAccounts_TransfersFundsAndRaisesEvent()
    {
        var transferService = new TransferDomainService();

        var sender = CreateCustomer();
        var reciver = new Customer(
                    "stoiko12",
                    "Stoiko",
                    "Gosshob",
                    new PhoneNumber("+359888488188"),
                    new Address("Smokinq", "Varna", 1000, "BG"),
                    new EGN("8102042087", new DateOnly(1990, 1, 1), Gender.Male)
                );

 var fromAccount = sender.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);
        var toAccount = reciver.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);
        sender.ClearDomainEvents();


        Assert.Equal(300m, fromAccount.Balance);
        Assert.Equal(300m, toAccount.Balance);

        var transferEvent = Assert.Single(sender.DomainEvents) as TransferInitiatedEvent;
        Assert.NotNull(transferEvent);
        Assert.Equal(fromAccount.Id, transferEvent.fromAccountId);
        Assert.Equal(toAccount.Id, transferEvent.toAccountId);
        Assert.Equal(200m, transferEvent.amount);
    }


    [Theory]
    [InlineData(true, false)]  // From non-existent
    [InlineData(false, true)]  // To non-existent
    public void Transfer_NonExistentAccount_ThrowsAccountNotFoundException(bool fromExists, bool toExists)
    {
        var customer = CreateCustomer();
        var existingAccount = customer.OpenAccount(AccountType.Checking, 500m, _ibanGenerator, _factory);

        var fromId = fromExists ? existingAccount.Id : Guid.NewGuid();
        var toId = toExists ? existingAccount.Id : Guid.NewGuid();

        Assert.Throws<AccountNotFoundException>(() =>
            customer.Transfer(fromId, toId, 50m));
    }


    [Fact]
    public void Transfer_InsufficientFunds_ThrowsInsufficientFundsException()
    {
        var customer = CreateCustomer();
        var fromAccount = customer.OpenAccount(AccountType.Checking, 100m, _ibanGenerator, _factory);
        var toAccount = customer.OpenAccount(AccountType.Checking, 50m, _ibanGenerator, _factory);

        Assert.Throws<InsufficientFundsException>(() =>
            customer.Transfer(fromAccount.Id, toAccount.Id, 200m));
    }


    #endregion

    #region GetAccountById Tests

    [Fact]
    public void GetAccountById_ExistingAccount_ReturnsAccount()
    {
        var (customer, account) = CreateCustomerWithAccount();

        var retrieved = customer.GetAccountById(account.Id);

        Assert.Equal(account.Id, retrieved.Id);
        Assert.Equal(account.IBAN, retrieved.IBAN);
    }

    [Fact]
    public void GetAccountById_NonExistentAccount_ThrowsAccountNotFoundException()
    {
        var customer = CreateCustomer();

        Assert.Throws<AccountNotFoundException>(() =>
            customer.GetAccountById(Guid.NewGuid()));
    }

    #endregion

    #region UpdateAddress Tests

    [Fact]
    public void UpdateAddress_ValidAddress_UpdatesSuccessfully()
    {
        var customer = CreateCustomer();

        customer.UpdateAddress("456 New St", "Plovdiv", 4000, "Bulgaria");

        Assert.Equal("456 New St", customer.Address.CityAddress);
        Assert.Equal("Plovdiv", customer.Address.City);
        Assert.Equal(4000, customer.Address.Zip);
        Assert.Equal("Bulgaria", customer.Address.Country);
    }

    #endregion

    #region UpdatePhoneNumber Tests

    [Theory]
    [InlineData("9876543210", true)]
    [InlineData("123", false)]
    public void UpdatePhoneNumber_ValidatesInput(string phoneNumber, bool shouldSucceed)
    {
        var customer = CreateCustomer();

        if (shouldSucceed)
        {
            customer.UpdatePhoneNumber(phoneNumber);
            Assert.Equal(phoneNumber, customer.PhoneNumber.Value);
        }
        else
        {
            Assert.Throws<InvalidPhoneNumberException>(() =>
                customer.UpdatePhoneNumber(phoneNumber));
        }
    }

    #endregion

    #region Deactivate Tests

    [Fact]
    public void Deactivate_NoAccounts_DeactivatesSuccessfully()
    {
        var customer = CreateCustomer();

        customer.Deactivate();

        Assert.Equal(CustomerStatus.Inactive, customer.Status);
    }

    [Fact]
    public void Deactivate_ClosedAccounts_DeactivatesSuccessfully()
    {
        var (customer, account) = CreateCustomerWithAccount();
        account.Withdraw(100);
        account.Close();

        customer.Deactivate();

        Assert.Equal(CustomerStatus.Inactive, customer.Status);
    }

    [Fact]
    public void Deactivate_FrozenAccounts_DeactivatesSuccessfully()
    {
        var (customer, account) = CreateCustomerWithAccount();
        account.Freeze();

        customer.Deactivate();

        Assert.Equal(CustomerStatus.Inactive, customer.Status);
    }

    [Fact]
    public void Deactivate_ActiveAccounts_ThrowsCannotDeactivateCustomerWithActiveAccountsException()
    {
        var (customer, _) = CreateCustomerWithAccount();

        Assert.Throws<CannotDeactivateCustomerWithActiveAccountsException>(() =>
            customer.Deactivate());
    }

    #endregion
}