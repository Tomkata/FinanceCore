namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    public record TransferBankAccountCommand(Guid customerId,Guid fromAccountId,Guid toAccountid, decimal amount);
}
