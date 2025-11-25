
namespace BankingSystem.Domain.Interfaces
{
    using BankingSystem.Domain.Entities;

    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id);
        Task SaveAsync(Account account);

    }
}
    