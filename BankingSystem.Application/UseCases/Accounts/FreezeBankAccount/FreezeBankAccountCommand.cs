namespace BankingSystem.Application.UseCases.Accounts.CloseBankAccount
{
    public record FreezeBankAccountCommand(Guid CustomerId, Guid AccountId);
}
