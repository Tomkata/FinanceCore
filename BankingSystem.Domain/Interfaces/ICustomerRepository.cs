
namespace BankingSystem.Domain.Interfaces
{
    using BankingSystem.Domain.Aggregates.Customer;

    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);

        Task SaveAsync(Customer account);

        Task<Customer?> FindByEgnAsync(string egn);


    }
}
