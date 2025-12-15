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
        public TransactionEntry(Guid accountId,LedgerAccountType ledgerAccountType, EntryType entryType,
            decimal amount)
        {
            if (accountId == Guid.Empty)
                throw new TransactionException("Account ID is required for TransactionEntry");

            if (amount == 0)
                throw new TransactionException("Transaction entry amount cannot be zero");

            var isIncrease = (ledgerAccountType == LedgerAccountType.Asset && entryType == EntryType.Debit)
             || (ledgerAccountType == LedgerAccountType.Liability && entryType == EntryType.Credit);

              


            AccountId = accountId;
            EntryType = entryType;
            Amount = isIncrease ? amount : -Math.Abs(amount);
        }



        public Guid AccountId { get; private set; }
        public Account Account { get; private set; }
        public EntryType EntryType { get; private set; }
        public decimal Amount { get; private set; }

        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }

    }
}
