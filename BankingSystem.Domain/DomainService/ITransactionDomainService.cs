using BankingSystem.Domain.Aggregates.Transaction;

namespace BankingSystem.Domain.DomainServices
{
    public interface ITransactionDomainService
    {
        Transaction CreateDepositTransaction(Guid accountId, decimal amount);
        Transaction CreateWithdrawTransaction(Guid accountId, decimal amount);
        Transaction CreateTransferTransaction(Guid fromAccountId, Guid toAccountId, decimal amount);
    }
}
