namespace BankingSystem.Domain.DomainService
{
    using BankingSystem.Domain.Aggregates.Customer;

    /// <summary>
    /// Domain service for handling transfers between customer aggregates.
    /// This service coordinates operations across aggregate boundaries.
    /// </summary>
    public interface ITransferDomainService
    {
        /// <summary>
        /// Transfers money between accounts belonging to different customers.
        /// Respects aggregate boundaries by operating on both Customer aggregates.
        /// </summary>
        void TransferBetweenCustomers(
            Customer sender,
            Customer receiver,
            Guid fromAccountId,
            Guid toAccountId,
            decimal amount);
    }
}
