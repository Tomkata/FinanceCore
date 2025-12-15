
namespace BankingSystem.Domain.Aggregates.Transaction
{
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.Enums;
    using BankingSystem.Domain.Enums.Account;
    using BankingSystem.Domain.Enums.Transaction;
    using System.Transactions;
    using TransactionStatus = Enums.Transaction.TransactionStatus;

    public class Transaction : AggregateRoot
    {
        private Transaction()
        {
        }

            private Transaction(
        TransactionType transactionType,
        TransactionStatus transactionStatus,
        string description)
        {
            TransactionType = transactionType;
            TransactionStatus = transactionStatus;
            Description = description;
            TransactionDate = DateTime.UtcNow;
        }

        public TransactionType TransactionType { get; private set; }
        public TransactionStatus TransactionStatus { get; private set; }

        public string Description { get;private set; }
        public DateTime TransactionDate { get; private set; }
                
        public virtual ICollection<TransactionEntry> TransactionEntries { get; private set; } = new HashSet<TransactionEntry>();

        public static Transaction Create(
     TransactionType type,
     string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new TransactionException("Transaction description is required.");

            return new Transaction(type, TransactionStatus.Pending, description);
        }

        public void AddEntry(EntryType type,Guid accountId, decimal amount, LedgerAccountType ledgerAccountType)
        {

            if (amount == 0)
                throw new TransactionException("Transaction entry amount cannot be zero");

           

            TransactionEntries.Add(new TransactionEntry(accountId, ledgerAccountType, type, amount));
        }


        public void Complete()
        {
            if (TransactionEntries.Count < 2)
                throw new TransactionException("Transaction must have at least two entries (double-entry rule).");

            if (TransactionEntries.Sum(x => x.Amount) != 0)
                throw new TransactionException("Unbalanced transaction (sum must equal 0).");

            if (TransactionStatus == TransactionStatus.Completed)
                throw new TransactionException("Transaction is already completed.");

           
            TransactionStatus = TransactionStatus.Completed;
        }

    }
}
