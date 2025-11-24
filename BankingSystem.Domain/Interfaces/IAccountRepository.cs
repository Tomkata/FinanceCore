
namespace BankingSystem.Domain.Interfaces
{
    using BankingSystem.Domain.Entities;

    public interface IAccountRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task SaveAsync(Customer account);

    }
}
