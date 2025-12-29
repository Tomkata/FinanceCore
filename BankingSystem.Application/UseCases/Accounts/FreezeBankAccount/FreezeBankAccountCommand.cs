namespace BankingSystem.Application.UseCases.Accounts.CloseBankAccount
{
    public record FreezeBankAccountCommand(Guid customerId, Guid accountId);
}
