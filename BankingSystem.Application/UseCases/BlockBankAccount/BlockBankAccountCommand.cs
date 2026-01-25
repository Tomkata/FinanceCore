namespace BankingSystem.Application.UseCases.BlockBankAccount
{
    public record BlockBankAccountCommand(Guid CustomerId, Guid AccountId);
}
