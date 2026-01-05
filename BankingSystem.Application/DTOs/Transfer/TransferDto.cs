namespace BankingSystem.Application.DTOs.Transfer
{
    public class TransferDto
    {
        public Guid CustomerId { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
