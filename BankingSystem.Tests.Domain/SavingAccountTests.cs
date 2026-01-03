using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BankingSystem.Tests.Domain
{
    public class SavingAccountTests
    {
        private IBAN Iban() => IBAN.Create("BG80BNBG96611020345678");

        [Fact]
        public void Constructor_Should_SetValues_AndStartAtZeroWithdrawals()
        {
            var account = new SavingAccount(Iban(), Guid.NewGuid(), withdrawLimit: 3);

            account.WithdrawLimits.Should().Be(3);
            account.CurrentMonthWithdrawals.Should().Be(0);
            account.LastWithdrawalDate.Should().BeNull();
            account.AccountType.Should().Be(AccountType.Saving);
        }

        [Fact]
        public void Constructor_InvalidWithdrawLimit_ShouldThrow()
        {
            Action act = () => new SavingAccount(Iban(), Guid.NewGuid(), withdrawLimit: 0);

            act.Should().Throw<AccountWithdrawInvalidParameter>();
        }


        [Fact]
        public void Withdraw_Should_DecreaseBalance_AndIncreaseWithdrawalCount()
        {
            var account = new SavingAccount(Iban(), Guid.NewGuid(), 3);
            account.Deposit(500);

            account.Withdraw(100);

            account.Balance.Should().Be(400);
            account.CurrentMonthWithdrawals.Should().Be(1);
        }

        [Fact]
        public void Withdraw_ExceedLimit_ShouldThrow()
        {
            var account = new SavingAccount(Iban(), Guid.NewGuid(), 2);
            account.Deposit(500);

            account.Withdraw(100);
            account.Withdraw(100);

            Action act = () => account.Withdraw(100);

            act.Should().Throw<AccountWithdrawLimitException>();
        }

        [Fact]
        public void Withdraw_NewMonth_ShouldResetWithdrawalCount()
        {
            var account = new SavingAccount(Iban(), Guid.NewGuid(), 1);
            account.Deposit(500);

            // First withdrawal of the month
            account.Withdraw(100);

            // Simulate last month
            account.GetType()
                .GetProperty("LastWithdrawalDate")!
                .SetValue(account, DateTime.UtcNow.AddMonths(-1));

            // Should reset and allow again
            Action act = () => account.Withdraw(100);

            act.Should().NotThrow();
            account.CurrentMonthWithdrawals.Should().Be(1);
        }
    }
}
