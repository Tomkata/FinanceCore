
namespace BankingSystem.Domain.Interfaces
{
    using BankingSystem.Domain.Entities;

    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);

        Task SaveAsync(Customer account);


    }
}
