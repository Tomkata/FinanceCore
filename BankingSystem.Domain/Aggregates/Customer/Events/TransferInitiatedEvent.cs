using BankingSystem.Domain.Common;

namespace BankingSystem.Domain.Aggregates.Customer.Events
{
    public record TransferInitiatedEvent(
    Guid customerId,
    Guid fromAccountId,
    Guid toAccountId,
    decimal amount) : IDomainEvent;
}
