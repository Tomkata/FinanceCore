using BankingSystem.Domain.Aggregates.Customer.Events;
using BankingSystem.Domain.Common;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Application.DomainEventHandlers
{
    public class AccountDebitedEventHandler : IDomainEventHandler<AccountDebitedEvent>
    {
        private readonly ITransactionDomainService _transactionDomainService;
        private readonly ITransactionRepository _transactionRepository;

        public AccountDebitedEventHandler(ITransactionDomainService transactionDomainService,
            ITransactionRepository transactionRepository)
        {
            this._transactionDomainService = transactionDomainService;
            this._transactionRepository = transactionRepository;
        }

        public Task Handle(AccountDebitedEvent doaminEvent, CancellationToken cancellationToken)
        {
            var transaction = _transactionDomainService
                .CreateWithdrawTransaction(doaminEvent.AccountId, doaminEvent.Amount);

            _transactionRepository.SaveAsync(transaction);

            return Task.CompletedTask;
        }
    }
}
