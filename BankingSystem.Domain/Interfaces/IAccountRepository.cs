
namespace BankingSystem.Domain.Interfaces
{
    using BankingSystem.Domain.Aggregates.Customer;

    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id);
        Task SaveAsync(Account account);

    }
}
    