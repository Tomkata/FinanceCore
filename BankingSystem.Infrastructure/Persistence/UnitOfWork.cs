

namespace BankingSystem.Infrastructure.Persistence
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Domain.Common;
    using BankingSystem.Infrastructure.Data;
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        private readonly IDomainEventDispatcher _dispatcher;

        public UnitOfWork(ApplicationDbContext context,
                            IDomainEventDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        public async Task SaveChangesAsync()
        {
            // 1. collect domain events before save
            var domainEvents = _context.ChangeTracker
                .Entries<AggregateRoot>()
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            // 2. save changes to DB
            await _context.SaveChangesAsync();

            // 3. dispatch events
            await _dispatcher.Dispatch(domainEvents);

            // 4. clear events
            foreach (var entry in _context.ChangeTracker.Entries<AggregateRoot>())
                entry.Entity.ClearDomainEvents();
        }
    }
}
