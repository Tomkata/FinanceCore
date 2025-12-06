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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return _context.Transactions
                               .AsNoTracking()
                               .Include(x => x.TransactionEntries)
                               .ToList();
        }

        public async Task SaveAsync(Transaction transaction)
        {
            var existingTransaction = await _context.Transactions
             .AsNoTracking()
             .FirstOrDefaultAsync(c => c.Id == transaction.Id);

            if (existingTransaction is null)
            {
                await _context.Transactions.AddAsync(transaction);
            }
            else
            {
                _context.Transactions.Update(transaction);
            }
        }
    }
}
