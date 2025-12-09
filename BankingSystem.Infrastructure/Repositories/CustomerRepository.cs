using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> FindByEgnAsync(string egn)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.EGN.Value == egn);
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _context.Customers.FindAsync(id);  
        }

        public async Task SaveAsync(Customer customer)  
        {
            var existingCustomer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customer.Id);

            if (existingCustomer is null)
            {
                await _context.Customers.AddAsync(customer);
            }
            else
            {
                _context.Customers.Update(customer);
            }
        }
    }
}
