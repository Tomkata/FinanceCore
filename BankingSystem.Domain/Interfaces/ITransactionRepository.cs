
using BankingSystem.Domain.Aggregates.Transaction;

namespace BankingSystem.Domain.Interfaces
{

using Transaction = Transaction;

    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(Guid id);
        Task SaveAsync(Transaction account);
        IQueryable<Transaction> Query();

        void Add(Transaction transaction);
    }
}
