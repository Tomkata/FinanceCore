using BankingSystem.Application.UseCases.Customers.GetCustomerById;
using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Tests.Domain
{
    public class AccountTests
    {

        [Fact]
        public void Deposit_Should_Increase_Balance()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678"); 
            var account = Account.CreateRegular(iban, customerId);

            var amount = 100m;

            // Act
            account.Deposit(amount);

            // Assert
            Assert.Equal(100m, account.Balance);
        }

        [Fact]
        public void Deposit_NegativeAmount_ShouldFail()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678");
            var account = Account.CreateRegular(iban, customerId);

            var amount = -100m;

            

            Assert.Throws<InvalidAmountException>(()=> account.Deposit(amount));
        }


        [Fact]
        public void Deposit_OnClosedAccount_ShouldFail()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678");
            var account = Account.CreateRegular(iban, customerId);

            account.Close();

            var amount = 100m;



            Assert.Throws<AccountNotActiveException>(() => account.Deposit(amount));
        }


        [Fact]
        public void Withdraw_WhenAmountExceedsBalance_ShouldThrow_AndNotChangeBalance()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678");
            var account = Account.CreateRegular(iban, customerId);

            var initialBalance = 100m;
            var withdrawAmount = 200m;

            account.Deposit(initialBalance);

            var ex = Assert.Throws<InsufficientFundsException>(() => account.Withdraw(withdrawAmount));

            Assert.Equal(initialBalance, account.Balance);
            Assert.Equal(withdrawAmount, ex.AttemptedAmount);
        }

        [Fact]
        public void Withdraw_BeforeMaturityDate_OnDepositAccount_ShouldThrowEarlyWithdrawalException_AndNotChangeBalance()
        {
            var customerId = Guid.NewGuid();
            var iban = IBAN.Create("BG80BNBG96611020345678");

            var initialBalance = 500m;
            var withdrawAmount = 100m;

            var account = Account.CreateDeposit(iban, customerId, new DepositTerm(12));
            account.Deposit(initialBalance);

            var ex = Assert.Throws<EarlyWithdrawalException>(() => account.Withdraw(withdrawAmount));

            Assert.Equal(account.MaturityDate, ex.MaturityDate);
            Assert.Equal(initialBalance, account.Balance);
        }


    }
}
