namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    public record TransferBankAccountCommand(
     Guid senderCustomerId,
     Guid receiverCustomerId,
     Guid fromAccountId,
     Guid toAccountId,
     decimal amount);
}
