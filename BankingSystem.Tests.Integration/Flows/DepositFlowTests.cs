using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Customer;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Services;

namespace BankingSystem.Tests.Domain;

public class CustomerTests
{
    private readonly FakeIbanGenerator _ibanGenerator = new();

    private Customer CreateCustomer(CustomerStatus? status = null)
    {
        var customer = new Customer(

            "testuser",
            "John",
            "Doe",
            new PhoneNumber("1234567890"),
            new Address("Main St", "Sofia", 1000, "Bulgaria"),
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
        var account = customer.OpenAccount(type, balance, _ibanGenerator, withdrawLimit, depositTerm);
        customer.ClearDomainEvents();
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
    public void OpenAccount_Cheking_CreatesAccountSuccessfully(AccountType type, decimal balance)
    {
        var customer = CreateCustomer();

        var account = customer.OpenAccount(type,balance,_ibanGenerator);

        Assert.NotNull(account);
        Assert.Equal(type, account.AccountType);
        Assert.Equal(balance, account.Balance);
        Assert.Equal(customer.Id, account.CustomerId);
        Assert.Single(customer.Accounts);
    }

    [Fact]
    public void OpenAccount_Saving_WithWithdrawLimit_CreatesAccountSuccessfully()
    {
        var customer = CreateCustomer();

        var account = customer.OpenAccount(AccountType.Saving, 500m, _ibanGenerator, withdrawLimit: 3);

        Assert.Equal(AccountType.Saving, account.AccountType);
        Assert.Equal(3, account.WithdrawLimits);
    }


    [Fact]
    public void OpenAccount_Saving_WithoutWithWithdrawLimit_CreatesAccountSuccessfully()
    {
        var customer = CreateCustomer();

        var ex = Assert.Throws<InvalidOperationException>(() => customer.OpenAccount(AccountType.Saving, 100m, _ibanGenerator));

        Assert.Contains("Withdraw limit required", ex.Message);

    }

    [Fact]
    public void OpenAccount_Deposit_WithDepositTerm_CreatesAccountWithMaturityDate()
    {
        var customer = CreateCustomer();

        var account = customer.OpenAccount(AccountType.Deposit, 1000m, _ibanGenerator,depositTerm: new DepositTerm(12));

        Assert.Equal(AccountType.Deposit,account.AccountType);
        Assert.NotNull(account.MaturityDate);
        Assert.True(account.MaturityDate > DateTime.UtcNow);

    }


    [Fact]
    public void OpenAccount_Deposit_WithoutDepositTerm_ThrowsInvalidOperationException()
    {
        var customer = CreateCustomer();

        var ex = Assert.Throws<InvalidOperationException>(()=>customer.OpenAccount(AccountType.Deposit, 1000m, _ibanGenerator));

        Assert.Contains("Withdraw limit required", ex.Message);
    }

    [Fact]
    public void OpenAccount_MultiplueCheckingAccounts_Succeeds()
    {
        var customer = CreateCustomer();

         customer.OpenAccount(AccountType.Checking, 200m, _ibanGenerator);
         customer.OpenAccount(AccountType.Checking, 100m, _ibanGenerator);

        Assert.Equal(2,customer.Accounts.Count);
    }

    [Fact]
    public void OpenAccount_MultipleDepositAccounts_ThrowsCannotHaveMultipleDepositAccountsException()
    {
        var customer = CreateCustomer();
        var depositTerm = new DepositTerm(12);
        customer.OpenAccount(AccountType.Deposit, 1000m, _ibanGenerator, depositTerm: depositTerm);

        Assert.Throws<CannotHaveMultipleDepositAccountsException>(() =>
            customer.OpenAccount(AccountType.Deposit, 2000m, _ibanGenerator, depositTerm: depositTerm));




    }

    #endregion


}
