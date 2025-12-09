using BankingSystem.Domain.Aggregates.Customer.Events;
using BankingSystem.Domain.Common;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.DomainEventHandlers
{
    public class TransferInitiatedEventHandler : IDomainEventHandler<TransferInitiatedEvent>
    {
        private readonly ITransactionDomainService _transactionService;
        private readonly ITransactionRepository _transactionRepository;

        public TransferInitiatedEventHandler(ITransactionDomainService transactionService,
            ITransactionRepository transactionRepository)
        {
           this._transactionService = transactionService;
           this._transactionRepository = transactionRepository;
        }

        public Task Handle(TransferInitiatedEvent doaminEvent, CancellationToken cancellationToken)
        {
            var transaction = _transactionService
                .CreateTransferTransaction(doaminEvent.fromAccountId,
                doaminEvent.toAccountId,
                doaminEvent.amount);

            _transactionRepository.Add(transaction);

            return Task.CompletedTask;
        }
    }
}
