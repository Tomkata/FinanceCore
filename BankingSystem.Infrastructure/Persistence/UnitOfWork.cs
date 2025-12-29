    namespace BankingSystem.Infrastructure.Persistence
    {
        using BankingSystem.Application.Common.Interfaces;
        using BankingSystem.Domain.Aggregates.Customer;
        using BankingSystem.Domain.Common;
        using BankingSystem.Infrastructure.Data;
        using Microsoft.EntityFrameworkCore;

        public class UnitOfWork : IUnitOfWork
        {
            private readonly ApplicationDbContext _context;
            private readonly IDomainEventDispatcher _dispatcher;

            public UnitOfWork(ApplicationDbContext context, IDomainEventDispatcher dispatcher)
            {
                _context = context;
                _dispatcher = dispatcher;
            }

            public async Task SaveChangesAsync()
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var entries = _context.ChangeTracker.Entries().ToList();
               
                    var aggregates = _context.ChangeTracker.Entries()
                        .Where(e => e.Entity is AggregateRoot)
                        .Select(e => (AggregateRoot)e.Entity)
                        .ToList();

                    var domainEvents = aggregates
                        .Where(a => a.DomainEvents != null && a.DomainEvents.Any())
                        .SelectMany(a => a.DomainEvents)
                        .ToList();

                    aggregates.ForEach(a => a.ClearDomainEvents());

                    await _context.SaveChangesAsync();


                    if (domainEvents.Any()) 
                    {
                        await _dispatcher.Dispatch(domainEvents);
                        
                        await _context.SaveChangesAsync();
                    }
                  

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }