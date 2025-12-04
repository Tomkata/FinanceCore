using BankingSystem.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BankingSystem.Application.DTOs.Transaction
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string TransactionType { get; set; }
        public string TransactionStatus { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }

        public List<TransactionEntryDto>  Entries { get; set; }

    }
}
