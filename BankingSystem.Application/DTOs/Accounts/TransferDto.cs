using System;

namespace BankingSystem.Application.DTOs.Accounts
{
    public class TransferDto
    {
        public Guid CustomerId { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
