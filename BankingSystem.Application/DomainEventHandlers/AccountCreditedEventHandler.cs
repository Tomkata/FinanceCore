
namespace BankingSystem.Application.DomainEventHandlers
{
    using BankingSystem.Domain.Aggregates.Customer.Events;
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.DomainServices;
    using BankingSystem.Domain.Interfaces;

    public class AccountCreditedEventHandler : IDomainEventHandler<AccountCreditedEvent>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionDomainService _transactionService;

        public AccountCreditedEventHandler(ITransactionRepository transactionRepository, 
            ITransactionDomainService transactionService)
        {
            this._transactionRepository = transactionRepository;
            this._transactionService = transactionService;
        }

        public Task Handle(AccountCreditedEvent domainEvent, CancellationToken cancellationToken)
        {
            var transaction = _transactionService.CreateDepositTransaction(domainEvent.accountId,domainEvent.amount);

            _transactionRepository.Add(transaction);

            return Task.CompletedTask;
        }
    }
}
