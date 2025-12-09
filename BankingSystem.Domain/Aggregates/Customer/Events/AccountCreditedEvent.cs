using BankingSystem.Domain.Common;

namespace BankingSystem.Domain.Aggregates.Customer.Events
{
    public record AccountCreditedEvent(
        Guid customerId,
        Guid accountId,
        decimal amount
        ) : IDomainEvent;
}
