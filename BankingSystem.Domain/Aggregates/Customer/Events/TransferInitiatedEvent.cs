using BankingSystem.Domain.Common;

namespace BankingSystem.Domain.Aggregates.Customer.Events
{
    public record TransferInitiatedEvent(
    Guid SenderCustomerId,
    Guid ReceiverCustomerId,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount) : IDomainEvent;
}
