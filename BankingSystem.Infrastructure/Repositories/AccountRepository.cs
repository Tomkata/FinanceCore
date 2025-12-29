using BankingSystem.Domain.Aggregates.Customer;
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
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext applicationDbContext)
        {
            this._context = applicationDbContext;
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts
                .AsNoTracking()
                .Include(x => x.Transactions)
                .ThenInclude(x => x.TransactionEntries)
                .FirstOrDefaultAsync(x => x.Id == id);  
        }

        public async Task<Account?> GetByIbanAsync(string iban)
        {
            return await _context.Accounts
                .AsNoTracking()
                .Include(x => x.Transactions)
                .ThenInclude(x => x.TransactionEntries)
                .FirstOrDefaultAsync(x => x.IBAN.Value == iban);    
        }

        public async Task SaveAsync(Account account)
            {
                var existingAccount = await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == account.Id);

                if (existingAccount is null)
                {
                    await _context.Accounts.AddAsync(account);
                }
                else
                {
                    _context.Accounts.Update(account);
                }

            }
    }
}
