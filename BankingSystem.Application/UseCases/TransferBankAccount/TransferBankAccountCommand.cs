namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    public record TransferBankAccountCommand(
        Guid SenderCustomerId,
        Guid ReceiverCustomerId,
        Guid FromAccountId,
        Guid ToAccountId,
        decimal Amount);
}
