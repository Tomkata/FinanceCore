using BankingSystem.Domain.Aggregates.Transaction;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Account;
using BankingSystem.Domain.Enums.Transaction;
using BankingSystem.Domain.Exceptions;
using FluentAssertions;
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

        #endregion

        #region Complete Tests


        #endregion
    }
}