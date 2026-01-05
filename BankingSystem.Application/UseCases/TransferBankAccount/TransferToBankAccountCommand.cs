namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    public record TransferToBankAccountCommand(Guid customerId,Guid fromAccountId,Guid toAccountid, decimal amount);
}
