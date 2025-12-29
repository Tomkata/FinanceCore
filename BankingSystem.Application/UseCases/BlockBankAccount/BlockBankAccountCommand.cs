namespace BankingSystem.Application.UseCases.BlockBankAccount
{
    public record BlockBankAccountCommand(Guid customerId, Guid accountId);
}
