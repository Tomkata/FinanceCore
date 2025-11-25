

namespace BankingSystem.Infrastructure.Presistence
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Infrastructure.Data;
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }   
    }
}
