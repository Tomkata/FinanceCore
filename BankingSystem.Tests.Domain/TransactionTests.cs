
using BankingSystem.Domain.Aggregates.Transaction;
using BankingSystem.Domain.Enums.Transaction;
using BankingSystem.Domain.Exceptions;
using FluentAssertions;
using System.Transactions;
using Transaction = BankingSystem.Domain.Aggregates.Transaction.Transaction;

namespace BankingSystem.Tests.Domain
{
    public class TransactionTests
    {
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
            Assert.Equal(BankingSystem.Domain.Enums.Transaction.TransactionStatus.Pending, transaction.TransactionStatus);
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
            var ex = Assert.Throws<TransactionException>(()=> 
            Transaction.Create(type, description));

            Assert.Contains("Transaction description is required",ex.Message);

        }

    }
}
