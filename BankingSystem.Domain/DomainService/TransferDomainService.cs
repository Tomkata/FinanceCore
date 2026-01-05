namespace BankingSystem.Domain.DomainService
{
    using BankingSystem.Domain.Aggregates.Customer;
    using BankingSystem.Domain.Aggregates.Customer.Events;
    using BankingSystem.Domain.Exceptions;

    /// <summary>
    /// Domain service for cross-customer transfers.
    ///
    /// WHY THIS EXISTS:
    /// - Transfer is a business operation spanning TWO aggregates (sender, receiver)
    /// - Customer aggregate should NOT know about other customers (aggregate boundary)
    /// - This service coordinates the operation while maintaining boundaries
    ///
    /// WHAT THIS IS NOT:
    /// - NOT a god class
    /// - NO state
    /// - NO repositories
    /// - NO persistence
    /// - PURE business logic
    /// </summary>
    public class TransferDomainService : ITransferDomainService
    {
        public void Transfer(
            Customer sender,
            Guid senderAccountId,
            Customer recipient,
            Guid recipientAccountId,
            decimal amount)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            if (recipient == null)
                throw new ArgumentNullException(nameof(recipient));

            // Business rule: cannot transfer to the same account
            if (senderAccountId == recipientAccountId)
                throw new CannotTransferToSameAccountException();

            // Execute operations on each aggregate
            sender.Withdraw(senderAccountId, amount);
            recipient.Deposit(recipientAccountId, amount);

            // Raise domain event from sender aggregate (initiator of the transfer)
            sender.AddDomainEvent(new TransferInitiatedEvent(
                sender.Id,
                recipient.Id,
                senderAccountId,
                recipientAccountId,
                amount
            ));
        }
    }
}
