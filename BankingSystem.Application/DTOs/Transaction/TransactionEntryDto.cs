namespace BankingSystem.Application.DTOs.Transaction
{
    public class TransactionEntryDto
    {
        public Guid AccountId { get; set; }
        public string EntryType { get; set; } 
        public decimal Amount { get; set; }
    }
}
