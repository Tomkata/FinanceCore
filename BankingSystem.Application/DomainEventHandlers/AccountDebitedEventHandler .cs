
namespace BankingSystem.Application.DomainEventHandlers
{
    using BankingSystem.Domain.Aggregates.Customer.Events;
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.DomainServices;
    using BankingSystem.Domain.Interfaces;

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
            var transaction =  _transactionDomainService
                .CreateWithdrawTransaction(doaminEvent.accountId, doaminEvent.amount);

            _transactionRepository.Add(transaction);

            return Task.CompletedTask;
        }
    }
}
