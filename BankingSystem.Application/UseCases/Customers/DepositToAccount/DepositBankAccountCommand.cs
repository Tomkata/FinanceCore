namespace BankingSystem.Application.UseCases.Customers.DepositToAccount
{
    public record DepositBankAccountCommand(Guid customerId, Guid accountId, decimal amount);
}
