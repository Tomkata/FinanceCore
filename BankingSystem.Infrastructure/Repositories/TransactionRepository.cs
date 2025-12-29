using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Aggregates.Transaction;
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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            return await _context.Transactions
                .Include(x=>x.TransactionEntries)
                .FirstOrDefaultAsync(t=>t.Id == id);
        }
        public void Add(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
        }

        public IQueryable<Transaction> Query()
        {
            return _context.Transactions
                .AsNoTracking()
                .Include(x => x.TransactionEntries);
        }

        public async Task SaveAsync(Transaction transaction)
        {
            var entry = _context.Entry(transaction);

            if (entry.State == EntityState.Detached)
            {
                var exists = await _context.Transactions
                    .AsNoTracking()
                    .AnyAsync(c => c.Id == transaction.Id);

                if (exists)
                    _context.Transactions.Update(transaction);
                else
                    await _context.Transactions.AddAsync(transaction);
            }
        }
    }
}
