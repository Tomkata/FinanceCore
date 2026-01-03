using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using FluentAssertions;

namespace BankingSystem.Tests.Integration.Accounts;

public class WithdrawTests
{
    private static readonly IBAN TestIban = IBAN.Create("BG80BNBG96611020345678");
    private readonly IAccountFactory _factory = new AccountFactory();


    private Account CreateRegularAccount(decimal initialDeposit = 0m)
    {
        var account = _factory.Create(
            AccountType.Checking,
            TestIban,
            Guid.NewGuid()
        );

        if (initialDeposit > 0)
            account.Deposit(initialDeposit);

        return account;
    }

    private SavingAccount CreateSavingAccount(int withdrawLimit, decimal initialDeposit = 0m)
    {
        var account = _factory.Create(
            AccountType.Saving,
            TestIban,
            Guid.NewGuid(),
            withdrawLimit: withdrawLimit
        ) as SavingAccount
          ?? throw new Exception("Factory did not return SavingAccount");

        if (initialDeposit > 0)
            account.Deposit(initialDeposit);

        return account;
    }

    private DepositAccount CreateDepositAccount(int termMonths, decimal initialDeposit = 0m)
    {
        var account = _factory.Create(
            AccountType.Deposit,
            TestIban,
            Guid.NewGuid(),
            depositTerm: new DepositTerm(termMonths)
        ) as DepositAccount
          ?? throw new Exception("Factory did not return DepositAccount");

        if (initialDeposit > 0)
            account.Deposit(initialDeposit);

        return account;
    }

    #region Regular Account Withdraw Tests

    [Theory]
    [InlineData(500, 200, 300)]
    [InlineData(250, 250, 0)]
    [InlineData(1000, 100, 900)]
    public void Withdraw_ValidAmount_DecreasesBalance(decimal initial, decimal withdraw, decimal expected)
    {
        var account = CreateRegularAccount(initial);

        account.Withdraw(withdraw);

        Assert.Equal(expected, account.Balance);
    }

    [Fact]
    public void Withdraw_MultipleWithdrawals_DecreasesBalanceCorrectly()
    {
        var account = CreateRegularAccount(1000m);

        account.Withdraw(100m);
        account.Withdraw(200m);
        account.Withdraw(150m);

        Assert.Equal(550m, account.Balance);
    }

    #endregion

    #region Invalid Amount Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    [InlineData(-100)]
    public void Withdraw_InvalidAmount_ThrowsInvalidAmountException(decimal amount)
    {
        var account = CreateRegularAccount(100m);

        Assert.Throws<InvalidAmountException>(() => account.Withdraw(amount));
    }

    [Fact]
    public void Withdraw_InsufficientFunds_ThrowsAndBalanceUnchanged()
    {
        var account = CreateRegularAccount(50m);

        Assert.Throws<InsufficientFundsException>(() => account.Withdraw(100m));
        Assert.Equal(50m, account.Balance);
    }

    #endregion

    #region Account Status Tests

    [Fact]
    public void Withdraw_FromClosedAccount_ThrowsAccountNotActiveException()
    {
        var account = CreateRegularAccount();
        account.Close();

        Assert.Throws<AccountNotActiveException>(() => account.Withdraw(50m));
    }

    [Fact]
    public void Withdraw_FromFrozenAccount_ThrowsAccountNotActiveException()
    {
        var account = CreateRegularAccount(100m);
        account.Freeze();

        Assert.Throws<AccountNotActiveException>(() => account.Withdraw(50m));
    }

    #endregion

    #region Saving Account Tests

    [Fact]
    public void Withdraw_FromSavingAccount_WithinLimit_Succeeds()
    {
        var account = CreateSavingAccount(withdrawLimit: 3, initialDeposit: 500m);

        account.Withdraw(100m);

        var saving = account as SavingAccount;
        saving.Should().NotBeNull();
        saving.CurrentMonthWithdrawals.Should().Be(1);
    }

    [Fact]
    public void Withdraw_FromSavingAccount_MultipleWithdrawalsWithinLimit_Succeeds()
    {
        var account = CreateSavingAccount(withdrawLimit: 3, initialDeposit: 500m);

        account.Withdraw(100m);
        account.Withdraw(50m);
        account.Withdraw(75m);

        Assert.Equal(275m, account.Balance);
        Assert.Equal(3, account.CurrentMonthWithdrawals);
    }

    [Fact]
    public void Withdraw_FromSavingAccount_ExceedingLimit_ThrowsAndBalanceUnchanged()
    {
        var account = CreateSavingAccount(withdrawLimit: 2, initialDeposit: 500m);
        account.Withdraw(100m);
        account.Withdraw(50m);

        Assert.Throws<AccountWithdrawLimitException>(() => account.Withdraw(25m));
        Assert.Equal(350m, account.Balance);
    }

    #endregion

    #region Deposit Account Tests

    [Fact]
    public void Withdraw_FromDepositAccount_AfterMaturity_Succeeds()
    {
        var account = CreateDepositAccount(termMonths: 1, initialDeposit: 1000m);
        var deposit = account as DepositAccount;
        deposit.Should().NotBeNull();

        typeof(DepositAccount)
            .GetProperty("MaturityDate")!
            .SetValue(deposit, DateTime.UtcNow.AddDays(-1));

        account.Withdraw(500m);

        Assert.Equal(500m, account.Balance);
    }


    #endregion
}