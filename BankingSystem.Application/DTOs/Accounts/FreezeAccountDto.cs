namespace BankingSystem.Application.DTOs.Accounts
{
    public class FreezeAccountDto
    {
        public Guid CustomerId { get; set; }
        public Guid AccountId { get; set; }
    }
}
