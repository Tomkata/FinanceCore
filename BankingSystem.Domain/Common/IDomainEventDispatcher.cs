namespace BankingSystem.Domain.Common
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(IEnumerable<IDomainEvent> events);
    }
}
