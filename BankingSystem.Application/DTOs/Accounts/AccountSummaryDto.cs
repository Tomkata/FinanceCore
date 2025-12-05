using BankingSystem.Application.DTOs.Transaction;
using System.Transactions;

namespace BankingSystem.Application.DTOs.Accounts
{
    public class AccountSummaryDto
    {
        public Guid Id { get; set; }
        public string IBAN { get; set; }
        public decimal Balance { get; set; }
        public string AccountType { get; set; }
        public string AccountStatus { get; set; }


    }
}
