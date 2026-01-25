namespace BankingSystem.Application.DTOs.Accounts
{
    public class ReactivateAccountDto
    {
        public Guid CustomerId { get; set; }
        public Guid AccountId { get; set; }
    }
}
