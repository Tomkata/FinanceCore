namespace BankingSystem.Domain.Aggregates.Transaction
{
    using BankingSystem.Domain.Aggregates.Customer;
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.Enums;
    using System.Transactions;

    public class TransactionEntry : BaseEntity
    {
        private TransactionEntry()
        {
            
        }
        public TransactionEntry(Guid accountId, EntryType entryType, decimal amount)
        {
            if (accountId == Guid.Empty)
                throw new TransactionException("Account ID is required for TransactionEntry");

            if (amount == 0)
                throw new TransactionException("Transaction entry amount cannot be zero");

            if (entryType == EntryType.Debit)
                amount = Math.Abs(amount);


            if (entryType == EntryType.Credit)
                amount = -Math.Abs(amount);

            AccountId = accountId;
            EntryType = entryType;
            Amount = amount;
        }



        public Guid AccountId { get; private set; }
        public Account Account { get; private set; }
        public EntryType EntryType { get; private set; }
        public decimal Amount { get; private set; }

        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }

    }
}
