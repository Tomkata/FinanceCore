using BankingSystem.Domain.Common;

namespace BankingSystem.Domain.Aggregates.Customer.Events
{
    public record AccountDebitedEvent(
     Guid customerId,
     Guid accountId,
     decimal amount
 ) : IDomainEvent;


}
