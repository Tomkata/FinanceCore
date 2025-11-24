
namespace BankingSystem.Domain.Interfaces
{

using Transaction = BankingSystem.Domain.Entities.Transaction;

    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(Guid id);
        Task SaveAsync(Transaction account);

    }
}
