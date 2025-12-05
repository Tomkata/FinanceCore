using BankingSystem.Application.DTOs.Transaction;

namespace BankingSystem.Application.DTOs.Accounts
{
    public class AccountWithRecentTransactionsDto
    {
        public Guid Id { get; set; }
        public string IBAN { get; set; }
        public decimal Balance { get; set; }
        public string AccountType { get; set; }
        public string AccountStatus { get; set; }


        public List<TransactionDto> RecentTransactions { get; set; }
    }
}
