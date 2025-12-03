namespace BankingSystem.Application.UseCases.Accounts.DepositBankAccount
{
    public record DepositBankAccountCommand(Guid customerId, Guid accountId, decimal amount);
}
