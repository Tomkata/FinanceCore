namespace BankingSystem.Application.UseCases.Customers.WithdrawFromAccount
{
    public record WithdrawBankAccountCommand(Guid customerId,decimal amount, Guid accountId);
}
