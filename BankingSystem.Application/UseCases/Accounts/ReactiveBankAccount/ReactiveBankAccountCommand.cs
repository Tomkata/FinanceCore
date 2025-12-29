namespace BankingSystem.Application.UseCases.Accounts.ReactiveBankAccount
{
    public record ReactiveBankAccountCommand(Guid customerId, Guid accountId);
}
