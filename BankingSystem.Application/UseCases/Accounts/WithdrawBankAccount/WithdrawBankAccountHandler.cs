using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.UseCases.Accounts.DepositBankAccount;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Application.UseCases.Accounts.WithdrawBankAccount
{
    public class WithdrawBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly WithdrawBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionDomainService _transactionDomainService;
        private readonly ITransactionRepository _transactionRepository;

        public WithdrawBankAccountHandler(ICustomerRepository customerRepository, 
            WithdrawBankAccountValidator validator,
            IUnitOfWork unitOfWork, 
            ITransactionDomainService transactionDomainService,
            ITransactionRepository transactionRepository)
        {
            this._customerRepository = customerRepository;
            this._validator = validator;
            this._unitOfWork = unitOfWork;
            this._transactionDomainService = transactionDomainService;
            this._transactionRepository = transactionRepository;
        }

        public async Task<Result<Guid>> Handle(WithdrawBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(String.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.customerId);

            if (customer is null)
                return Result<Guid>.Failure("Customer not found");

            var account = customer.GetAccountById(command.accountId);

            account.Withdraw(command.amount);

           var transaction =  _transactionDomainService.CreateWithdrawTransaction(account.Id,command.amount);
            await _transactionRepository.SaveAsync(transaction);

            await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(account.Id);

        }
    }
}
