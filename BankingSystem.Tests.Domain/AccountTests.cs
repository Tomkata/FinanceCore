using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Account;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using FluentAssertions;

namespace BankingSystem.Tests.Domain
{
    public class AccountTests
    {
        [Fact]
        public void Deposit_ShouldIncreaseBalance()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678");
            var account = Account.CreateRegular(iban, customerId);
            var amount = 100m;

            account.Deposit(amount);

            account.Balance.Should().Be(amount);
        }

        [Fact]
        public void Deposit_NegativeAmount_ShouldThrowInvalidAmountException()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678");
            var account = Account.CreateRegular(iban, customerId);
            var amount = -100m;

            var action = () => account.Deposit(amount);

            action.Should().Throw<InvalidAmountException>();
        }

        [Fact]
        public void Deposit_OnClosedAccount_ShouldThrowAccountNotActiveException()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678");
            var account = Account.CreateRegular(iban, customerId);
            account.Close();

            var amount = 100m;

            var action = () => account.Deposit(amount);

            action.Should().Throw<AccountNotActiveException>();
        }

        [Fact]
        public void Withdraw_WhenAmountExceedsBalance_ShouldThrowInsufficientFundsException_AndNotChangeBalance()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678");
            var account = Account.CreateRegular(iban, customerId);

            account.Deposit(100m);
            var withdrawAmount = 200m;

            var action = () => account.Withdraw(withdrawAmount);

            action.Should()
                  .Throw<InsufficientFundsException>()
                  .Which.AttemptedAmount.Should().Be(withdrawAmount);

            account.Balance.Should().Be(100m);
        }

        [Fact]
        public void Withdraw_BeforeMaturityDate_OnDepositAccount_ShouldThrowEarlyWithdrawalException_AndNotChangeBalance()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG05BNPA94401473621817");
            var account = Account.CreateDeposit(iban, customerId, new DepositTerm(12));

            account.Deposit(500m);
            var withdrawAmount = 100m;

            var action = () => account.Withdraw(withdrawAmount);

            action.Should()
                  .Throw<EarlyWithdrawalException>()
                  .Which.MaturityDate.Should().Be(account.MaturityDate);

            account.Balance.Should().Be(500m);
        }

        #region Factort Method Tests
        [Fact]
        public void CreateRegular_WithValidParameters_ShouldCreateCheckingAccount()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678");

            var account = Account.CreateRegular(iban,customerId);


            Assert.NotNull(account);
            Assert.Equal(AccountType.Checking, account.AccountType);
            Assert.Equal(iban, account.IBAN);
            Assert.Equal(customerId, account.CustomerId);
            Assert.Equal(0m, account.Balance);
            Assert.Equal(AccountStatus.Active, account.AccountStatus);
            Assert.Null(account.WithdrawLimits);
            Assert.Null(account.DepositTerm);
            Assert.Null(account.MaturityDate);
        }


        [Fact]

        public void CreateRegular_WithNullIban_ShouldThrowIbanNullException()

        {

            var customerId = Guid.NewGuid();


            // Act & Assert

            Assert.Throws<IbanException>(() => Account.CreateRegular(null!, customerId));

        }
        #endregion


    }
}
