namespace BankingSystem.Application.UseCases.Accounts.ReactiveBankAccount
{
    public record ReactiveBankAccountCommand(Guid CustomerId, Guid AccountId);
}
