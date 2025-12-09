using BankingSystem.Domain.Common;

namespace BankingSystem.Domain.Aggregates.Customer.Events
{
    public record AccountDebitedEvent(
     Guid CustomerId,
     Guid AccountId,
     decimal Amount
 ) : IDomainEvent;


}
