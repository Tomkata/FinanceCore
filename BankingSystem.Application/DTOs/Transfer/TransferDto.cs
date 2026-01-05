namespace BankingSystem.Application.DTOs.Transfer
{
    public class TransferDto
    {
        public Guid SenderCustomerId { get; set; }
        public Guid ReceiverCustomerId { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
