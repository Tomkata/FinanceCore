using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
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
