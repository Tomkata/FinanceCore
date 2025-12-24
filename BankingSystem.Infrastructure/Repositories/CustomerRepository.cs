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
            return await _context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task SaveAsync(Customer customer)
        {
            var entry = _context.Entry(customer);

            if (entry.State == EntityState.Detached)
            {
                var exists = await _context.Customers
                    .AsNoTracking()
                    .AnyAsync(c => c.Id == customer.Id);

                if (exists)
                    _context.Customers.Update(customer);
                else
                    await _context.Customers.AddAsync(customer);
            }
        }
    }
}
