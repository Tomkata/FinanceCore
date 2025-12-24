using BankingSystem.Domain.Enums;
using BankingSystem.Domain.ValueObjects;

namespace BankingSystem.Application.DTOs.Accounts
{
    public class OpenAccountDto
    {
        public int AccountType { get; set; }
        public Guid CustomerId { get; set; }
        public decimal InitialBalance { get; set; }
        public int? WithdrawLimit { get; set; }
        public int? DepositTerm { get; set; }
    }   
}
