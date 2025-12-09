using System.Transactions;

namespace BankingSystem.Domain.DomainService
{
    using Transaction = Aggregates.Transaction.Transaction;

    public interface ITransactionDomainService
    {
        Transaction CreateDepositTransaction(Guid accountId, decimal amount);
        Transaction CreateWithdrawTransaction(Guid accountId, decimal amount);
        Transaction CreateTransferTransaction(Guid fromAccountId, Guid toAccountId, decimal amount);
    }
}
