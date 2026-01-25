namespace BankingSystem.Application.DTOs.Accounts
{
    public class CloseAccountDto
    {
        public Guid CustomerId { get; set; }
        public Guid AccountId { get; set; }
    }
}
