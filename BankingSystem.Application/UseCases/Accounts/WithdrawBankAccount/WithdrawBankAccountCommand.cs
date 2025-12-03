namespace BankingSystem.Application.UseCases.Accounts.WithdrawBankAccount
{
    public record WithdrawBankAccountCommand(Guid customerId,decimal amount, Guid accountId);
}
