using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.UseCases.Accounts.DepositBankAccount;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.Aggregates;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    public class TransferBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly TransferBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionDomainService _transactionDomainService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository  _accountRepository;


        public TransferBankAccountHandler(ICustomerRepository customerRepository,
            TransferBankAccountValidator validator, 
            IUnitOfWork unitOfWork, 
            ITransactionDomainService transactionDomainService, 
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository)
        {
            this._customerRepository = customerRepository;
            this._validator = validator;
            this._unitOfWork = unitOfWork;
            this._transactionDomainService = transactionDomainService;
            this._transactionRepository = transactionRepository;
            this._accountRepository = accountRepository;
        }

        public async Task<Result<Guid>> Handle(TransferBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(String.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var senderCustomer = await _customerRepository.GetByIdAsync(command.customerId);

            if (senderCustomer is null)
                return Result<Guid>.Failure("Customer not found");


            var fromAccount =  senderCustomer.GetAccountById(command.fromAccountId);

            var toAccount = await _accountRepository.GetByIdAsync(command.toAccountid);
            if(toAccount is null)
                return Result<Guid>.Failure("Destination account not found");

            var receiverCustomer = await _customerRepository.GetByIdAsync(toAccount.CustomerId); 
            if (receiverCustomer is null) 
                   return Result<Guid>.Failure("Receiver customer not found");

            fromAccount.Withdraw(command.amount);
            toAccount.Deposit(command.amount);

            var transaction = _transactionDomainService.CreateTransferTransaction(
                fromAccount.Id,
                toAccount.Id,
                command.amount
            );

            await _transactionRepository.SaveAsync(transaction);

            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(transaction.Id);
        }
    }
}
