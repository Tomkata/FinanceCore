using BankingSystem.Domain.Aggregates.Transaction;
using BankingSystem.Domain.Enums.Transaction;

namespace BankingSystem.Domain.DomainServices
{
    using Transaction = Transaction;

    public class TransactionDomainService : ITransactionDomainService
    {
        private readonly Guid _bankVaultAccountId;

        public TransactionDomainService(Guid bankVaultAccountId)
        {
            _bankVaultAccountId = bankVaultAccountId;
        }

        public Transaction CreateDepositTransaction(Guid accountId, decimal amount)
        {
            var transaction = Transaction.Create(TransactionType.Deposit, $"DEPOSIT {amount}");
            transaction.AddEntry(Enums.EntryType.Credit, accountId, amount);
            transaction.AddEntry(Enums.EntryType.Debit, _bankVaultAccountId, -amount);  
            transaction.Complete();
            return transaction;
        }

        public Transaction CreateTransferTransaction(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var transaction = Transaction.Create(TransactionType.Transfer, $"TRANSFER {amount} FROM {fromAccountId} TO {toAccountId}");
            transaction.AddEntry(Enums.EntryType.Debit, fromAccountId, -amount);  //
            transaction.AddEntry(Enums.EntryType.Credit, toAccountId, amount);
            transaction.Complete();
            return transaction;
        }

        public Transaction CreateWithdrawTransaction(Guid accountId, decimal amount)
        {
            var transaction = Transaction.Create(TransactionType.Withdrawal, $"WITHDRAW {amount}");
            transaction.AddEntry(Enums.EntryType.Debit, accountId, -amount);  
            transaction.AddEntry(Enums.EntryType.Credit, _bankVaultAccountId, amount);
            transaction.Complete();
            return transaction;
        }
    }
}