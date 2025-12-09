namespace BankingSystem.Domain.Common
{
    public interface IDomainEventHandler<TEvent>
        where TEvent : IDomainEvent
    {
        Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
    }
}
