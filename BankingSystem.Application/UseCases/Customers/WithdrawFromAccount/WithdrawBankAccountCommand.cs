namespace BankingSystem.Application.UseCases.Customers.WithdrawFromAccount
{
    public record WithdrawBankAccountCommand(Guid CustomerId, Guid AccountId, decimal Amount);
}
