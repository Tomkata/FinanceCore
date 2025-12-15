namespace BankingSystem.Domain.Aggregates.Transaction
{
    using BankingSystem.Domain.Aggregates.Customer;
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.Enums;
    using BankingSystem.Domain.Enums.Account;
    using System.Transactions;

    public class TransactionEntry : BaseEntity
    {
        private TransactionEntry()
        {
            
        }
        public TransactionEntry(Guid accountId, LedgerAccountType ledgerAccountType, EntryType entryType, decimal amount)
        {
            if (accountId == Guid.Empty)
                throw new TransactionException("Account ID is required for TransactionEntry");
            if (amount <= 0)
                throw new TransactionException("Transaction entry amount must be positive");

            AccountId = accountId;
            EntryType = entryType;
            LedgerAccountType = ledgerAccountType;

            // Просто: Debit = +, Credit = -
            Amount = entryType == EntryType.Debit ? amount : -amount;
        }



        public Guid AccountId { get; private set; }
        public Account Account { get; private set; }
        public EntryType EntryType { get; private set; }
        public LedgerAccountType LedgerAccountType { get; set; }
        public decimal Amount { get; private set; }

        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }

    }
}
