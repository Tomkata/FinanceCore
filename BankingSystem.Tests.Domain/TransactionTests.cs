using BankingSystem.Domain.Aggregates.Transaction;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Account;
using BankingSystem.Domain.Enums.Transaction;
using BankingSystem.Domain.Exceptions;
using System.Transactions;
using Transaction = BankingSystem.Domain.Aggregates.Transaction.Transaction;
using TransactionStatus = BankingSystem.Domain.Enums.Transaction.TransactionStatus;

namespace BankingSystem.Tests.Domain
{
    public class TransactionTests
    {
        #region Transaction Create Tests

        [Theory]
        [InlineData(TransactionType.Deposit, "DEPOSIT")]
        [InlineData(TransactionType.Withdrawal, "WITHDRAW")]
        [InlineData(TransactionType.Transfer, "TRANSFER")]
        [InlineData(TransactionType.Fee, "FEE")]
        [InlineData(TransactionType.Payment, "PAYMENT")]
        public void Create_ValidTransactionType_Should_Be_Successful(
            TransactionType type,
            string description)
        {
            var transaction = Transaction.Create(type, description);

            Assert.Equal(type, transaction.TransactionType);
            Assert.Equal(description, transaction.Description);
            Assert.Equal(TransactionStatus.Pending, transaction.TransactionStatus);
            Assert.True(transaction.TransactionDate <= DateTime.UtcNow);
            Assert.True(transaction.TransactionDate > DateTime.UtcNow.AddSeconds(-1));
        }

        [Theory]
        [InlineData(TransactionType.Deposit, "  ")]
        [InlineData(TransactionType.Withdrawal, "")]
        [InlineData(TransactionType.Withdrawal, null)]
        public void Create_Transaction_With_Invalid_Description_Should_Not_Succeed(
            TransactionType type,
            string description)
        {
            var ex = Assert.Throws<TransactionException>(() =>
                Transaction.Create(type, description));

            Assert.Contains("Transaction description is required", ex.Message);
        }

        #endregion

        #region AddEntry Tests

        [Fact]
        public void AddEntry_Debit_Should_Have_Positive_Amount()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");
            var accountId = Guid.NewGuid();

            transaction.AddEntry(EntryType.Debit, accountId, 100m, LedgerAccountType.Asset);

            var entry = transaction.TransactionEntries.First();
            Assert.Equal(100m, entry.Amount);
            Assert.Equal(EntryType.Debit, entry.EntryType);
            Assert.Equal(accountId, entry.AccountId);
        }

        [Fact]
        public void AddEntry_Credit_Should_Have_Negative_Amount()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");
            var accountId = Guid.NewGuid();

            transaction.AddEntry(EntryType.Credit, accountId, 100m, LedgerAccountType.Liability);

            var entry = transaction.TransactionEntries.First();
            Assert.Equal(-100m, entry.Amount);
            Assert.Equal(EntryType.Credit, entry.EntryType);
            Assert.Equal(accountId, entry.AccountId);
        }

        [Fact]
        public void AddEntry_ZeroAmount_Should_Throw_Exception()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");

            var ex = Assert.Throws<TransactionException>(() =>
                transaction.AddEntry(EntryType.Debit, Guid.NewGuid(), 0m, LedgerAccountType.Asset));

            Assert.Contains("amount", ex.Message.ToLower());
        }

        [Fact]
        public void AddEntry_NegativeAmount_Should_Throw_Exception()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");

            var ex = Assert.Throws<TransactionException>(() =>
                transaction.AddEntry(EntryType.Debit, Guid.NewGuid(), -100m, LedgerAccountType.Asset));

            Assert.Contains("amount", ex.Message.ToLower());
        }

        [Fact]
        public void AddEntry_EmptyAccountId_Should_Throw_Exception()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");

            var ex = Assert.Throws<TransactionException>(() =>
                transaction.AddEntry(EntryType.Debit, Guid.Empty, 100m, LedgerAccountType.Asset));

            Assert.Contains("Account ID", ex.Message);
        }

        #endregion

        #region Complete Tests

        [Fact]
        public void Complete_BalancedTransaction_Should_Succeed()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");
            var vaultId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            transaction.AddEntry(EntryType.Debit, vaultId, 100m, LedgerAccountType.Asset);   
            transaction.AddEntry(EntryType.Credit, clientId, 100m, LedgerAccountType.Liability); 

            transaction.Complete();

            Assert.Equal(TransactionStatus.Completed, transaction.TransactionStatus);
        }

        [Fact]
        public void Complete_UnbalancedTransaction_Should_Throw_Exception()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");

            transaction.AddEntry(EntryType.Debit, Guid.NewGuid(), 100m, LedgerAccountType.Asset);
            transaction.AddEntry(EntryType.Debit, Guid.NewGuid(), 50m, LedgerAccountType.Asset);

            var ex = Assert.Throws<TransactionException>(() => transaction.Complete());

            Assert.Contains("Unbalanced", ex.Message);
        }

        [Fact]
        public void Complete_LessThanTwoEntries_Should_Throw_Exception()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");

            transaction.AddEntry(EntryType.Debit, Guid.NewGuid(), 100m, LedgerAccountType.Asset);

            var ex = Assert.Throws<TransactionException>(() => transaction.Complete());

            Assert.Contains("at least two entries", ex.Message);
        }

        [Fact]
        public void Complete_AlreadyCompleted_Should_Throw_Exception()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");

            transaction.AddEntry(EntryType.Debit, Guid.NewGuid(), 100m, LedgerAccountType.Asset);
            transaction.AddEntry(EntryType.Credit, Guid.NewGuid(), 100m, LedgerAccountType.Liability);
            transaction.Complete();

            var ex = Assert.Throws<TransactionException>(() => transaction.Complete());

            Assert.Contains("already completed", ex.Message);
        }

        [Fact]
        public void Complete_NoEntries_Should_Throw_Exception()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "TEST");

            var ex = Assert.Throws<TransactionException>(() => transaction.Complete());

            Assert.Contains("at least two entries", ex.Message);
        }

        #endregion

        #region Double-Entry Scenarios Tests

        [Fact]
        public void Deposit_Scenario_Should_Be_Balanced()
        {
            var transaction = Transaction.Create(TransactionType.Deposit, "DEPOSIT 500");
            var vaultId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            transaction.AddEntry(EntryType.Debit, vaultId, 500m, LedgerAccountType.Asset);     
            transaction.AddEntry(EntryType.Credit, clientId, 500m, LedgerAccountType.Liability); 

            transaction.Complete();

            Assert.Equal(TransactionStatus.Completed, transaction.TransactionStatus);
            Assert.Equal(0, transaction.TransactionEntries.Sum(e => e.Amount));
        }

        [Fact]
        public void Withdraw_Scenario_Should_Be_Balanced()
        {
            var transaction = Transaction.Create(TransactionType.Withdrawal, "WITHDRAW 200");
            var vaultId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            transaction.AddEntry(EntryType.Credit, vaultId, 200m, LedgerAccountType.Asset);   
            transaction.AddEntry(EntryType.Debit, clientId, 200m, LedgerAccountType.Liability); 

            transaction.Complete();

            Assert.Equal(TransactionStatus.Completed, transaction.TransactionStatus);
            Assert.Equal(0, transaction.TransactionEntries.Sum(e => e.Amount));
        }

        [Fact]
        public void Transfer_Scenario_Should_Be_Balanced()
        {
            var transaction = Transaction.Create(TransactionType.Transfer, "TRANSFER 300");
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();

            // Transfer: from намалява (Debit), to увеличава (Credit)
            transaction.AddEntry(EntryType.Debit, fromAccountId, 300m, LedgerAccountType.Liability);  
            transaction.AddEntry(EntryType.Credit, toAccountId, 300m, LedgerAccountType.Liability);   

            transaction.Complete();

            Assert.Equal(TransactionStatus.Completed, transaction.TransactionStatus);
            Assert.Equal(0, transaction.TransactionEntries.Sum(e => e.Amount));
        }

        #endregion
    }
}