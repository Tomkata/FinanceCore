
namespace BankingSystem.Domain.Entities
{
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.Enums;
    using BankingSystem.Domain.Enums.Transaction;
    using System.Transactions;
    using TransactionStatus = Enums.Transaction.TransactionStatus;

    public class Transaction : BaseEntity
    {
        private Transaction()
        {
        }

            private Transaction(
        TransactionType transactionType,
        TransactionStatus transactionStatus,
        string description,
        string idempotencyKey,
        Account account)
        {
            TransactionType = transactionType;
            TransactionStatus = transactionStatus;
            Description = description;
            IdempotencyKey = idempotencyKey;
            TransactionDate = DateTime.UtcNow;
            Account = account;
        }

        public TransactionType TransactionType { get; private set; }
        public TransactionStatus TransactionStatus { get; private set; }

        public string Description { get;private set; }
        public string IdempotencyKey { get; private set; }
        public DateTime TransactionDate { get; private set; }

        public virtual ICollection<TransactionEntry> TransactionEntries { get; private set; } = new HashSet<TransactionEntry>();

        public virtual Account Account { get; private set; }

        public static Transaction Create(
     TransactionType type,
     string description,
     string idempotencyKey,
     Account account)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new TransactionException("Transaction description is required.");

            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new TransactionException("Idempotency key is required.");

            if (account == null)
                throw new TransactionException("Transaction must be linked to an account.");

            return new Transaction(type, TransactionStatus.Pending, description, idempotencyKey, account);
        }

        public void AddEntry(EntryType type, decimal amount, Account account)
        {
            if (account == null)
                throw new TransactionException("Account is required for transaction entry");

            if (amount == 0)
                throw new TransactionException("Transaction entry amount cannot be zero");

            if (type == EntryType.Debit && amount > 0)
                throw new TransactionException("Debit entry must have negative amount");

            if (type == EntryType.Credit && amount < 0)
                throw new TransactionException("Credit entry must have positive amount");

            TransactionEntries.Add(new TransactionEntry(account.Id, type, amount));
        }


        public void Complete()
        {
            if (TransactionEntries.Count < 2)
                throw new TransactionException("Transaction must have at least two entries (double-entry rule).");

            if (TransactionEntries.Sum(x => x.Amount) != 0)
                throw new TransactionException("Unbalanced transaction (sum must equal 0).");

            if (TransactionStatus == TransactionStatus.Completed)
                throw new TransactionException("Transaction is already completed.");

            if (TransactionType == TransactionType.Withdrawal || TransactionType == TransactionType.Payment)
            {
                var outgoing = TransactionEntries.Where(e => e.Amount < 0).Sum(e => e.Amount);
                if (Account.Balance + outgoing < 0)
                    throw new TransactionException("Insufficient account balance.");
            }

            TransactionStatus = TransactionStatus.Completed;
        }

    }
}
