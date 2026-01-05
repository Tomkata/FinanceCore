using BankingSystem.Domain.Common;

namespace BankingSystem.Domain.Aggregates.Customer.Events
{
    public record TransferInitiatedEvent(
    Guid senderId,
    Guid recipentId,
    Guid fromAccountId,
    Guid toAccountId,
    decimal amount) : IDomainEvent;
}
