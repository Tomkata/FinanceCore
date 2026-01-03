using BankingSystem.Domain.Aggregates.Customer;
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
            new Address("Main St", "Sofia", 1000, "Bulgaria"),
            EGN.Create("0651035020")
        );

        if (status.HasValue && status != CustomerStatus.Active)
            typeof(Customer).GetProperty("Status")?.SetValue(customer, status.Value);

        return customer;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ValidParameters_CreatesActiveCustomerWithEmptyAccounts()
    {
        var customer = CreateCustomer();
        customer.Status.Should().Be(CustomerStatus.Active);
        customer.Accounts.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_NullEGN_ThrowsArgumentNullException()
    {
        Action act = () => new Customer(
            "testuser",
            "John",
            "Doe",
            new PhoneNumber("1234567890"),
            new Address("123 Main St", "Sofia", 1000, "Bulgaria"),
            null!
        );

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region OpenAccount Tests

    [Theory]
    [InlineData(AccountType.Checking, 100)]
    [InlineData(AccountType.Checking, 0)]
    public void OpenAccount_Checking_CreatesAccountSuccessfully(AccountType type, decimal balance)
    {
        var customer = CreateCustomer();
        var account = customer.OpenAccount(type, balance, _ibanGenerator, _factory);

        account.Should().NotBeNull();
        account.AccountType.Should().Be(type);
        account.Balance.Should().Be(balance);
        customer.Accounts.Should().Contain(account);
    }

    [Fact]
    public void OpenAccount_Saving_WithLimit_ShouldCreateSavingAccount()
    {
        var customer = CreateCustomer();
        var account = customer.OpenAccount(AccountType.Saving, 500, _ibanGenerator, _factory, withdrawLimit: 3);

        account.Should().BeOfType<SavingAccount>();
    }

    [Fact]
    public void OpenAccount_Saving_WithoutLimit_ShouldThrow()
    {
        var customer = CreateCustomer();
        Action act = () => customer.OpenAccount(AccountType.Saving, 100, _ibanGenerator, _factory);

        act.Should().Throw<WithdrawLimitRequiredException>();
    }

    [Fact]
    public void OpenAccount_Deposit_WithTerm_ShouldCreateDepositAccount()
    {
        var customer = CreateCustomer();
        var acc = customer.OpenAccount(
            AccountType.Deposit,
            1000,
            _ibanGenerator,
            _factory,
            depositTerm: new DepositTerm(12)
        );

        var depositAcc = acc.Should().BeOfType<DepositAccount>().Subject;

        depositAcc.MaturityDate.Should().BeAfter(DateTime.UtcNow);
    }



    [Fact]
    public void OpenAccount_Deposit_WithoutTerm_ShouldThrow()
    {
        var customer = CreateCustomer();
        Action act = () => customer.OpenAccount(AccountType.Deposit, 1000, _ibanGenerator, _factory);

        act.Should().Throw<DepositTermRequiredException>();
    }

    [Fact]
    public void OpenAccount_MultipleCheckingAccounts_ShouldWork()
    {
        var customer = CreateCustomer();
        customer.OpenAccount(AccountType.Checking, 200, _ibanGenerator, _factory);
        customer.OpenAccount(AccountType.Checking, 100, _ibanGenerator, _factory);

        customer.Accounts.Count.Should().Be(2);
    }

    [Fact]
    public void OpenAccount_MultipleDepositAccounts_ShouldThrow()
    {
        var customer = CreateCustomer();
        var term = new DepositTerm(12);

        customer.OpenAccount(AccountType.Deposit, 1000, _ibanGenerator, _factory, depositTerm: term);

        Action act = () => customer.OpenAccount(AccountType.Deposit, 2000, _ibanGenerator, _factory, depositTerm: term);
        act.Should().Throw<CannotHaveMultipleDepositAccountsException>();
    }

    #endregion
}
