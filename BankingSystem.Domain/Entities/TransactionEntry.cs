using BankingSystem.Domain.Common;
using BankingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Entities
{
    public class TransactionEntry : BaseEntity
    {
        private TransactionEntry()
        {
            
        }
        public TransactionEntry(Guid accountId, Guid transactionId, EntryType entryType, decimal amount)
        {
            AccountId = accountId;
            TransactionId = transactionId;
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
