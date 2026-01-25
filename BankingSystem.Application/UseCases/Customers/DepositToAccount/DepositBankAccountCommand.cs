namespace BankingSystem.Application.UseCases.Customers.DepositToAccount
{
    public record DepositBankAccountCommand(Guid CustomerId, Guid AccountId, decimal Amount);
}
