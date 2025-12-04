using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.UseCases.Accounts.CloseBankAccount;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.UseCases.Accounts.DepositBankAccount
{
    public class DepositBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly DepositBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionDomainService _transactionDomainService;
        private readonly ITransactionRepository _transactionRepository;

        public DepositBankAccountHandler(ICustomerRepository customerRepository,
            DepositBankAccountValidator validator,
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

        public async Task<Result<Guid>> Handle(DepositBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(String.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.customerId);

            if(customer is null)
                return Result<Guid>.Failure("Customer not found");

            var account = customer.GetAccountById(command.accountId);   

            account.Deposit(command.amount);

            var transaction = _transactionDomainService.CreateDepositTransaction(account.Id,command.amount);
            await _transactionRepository.SaveAsync(transaction);

            await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(account.Id);

        }
    }
}
