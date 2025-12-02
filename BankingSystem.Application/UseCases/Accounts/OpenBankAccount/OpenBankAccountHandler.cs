using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Domain.DomainServics;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.Interfaces;
using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;

namespace BankingSystem.Application.UseCases.Accounts.OpenBankAccount
{
    public class OpenBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly OpenBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIbanGenerator _ibanGenerator;

        public OpenBankAccountHandler(
                 ICustomerRepository customerRepository,
                 IAccountRepository accountRepository,
                 OpenBankAccountValidator validator,
                 IUnitOfWork unitOfWork,
                 IIbanGenerator ibanGenerator)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _ibanGenerator = ibanGenerator;
        }

        public async Task<Result<Guid>> Handle(OpenBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
                return Result<Guid>.Failure(string.Join(", ",validationResult.Errors.Select(x=>x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.customerId);

            if (customer is null)
                return Result<Guid>.Failure("Customer not found");


            var account = customer.OpenAccount(
                command.type, 
                command.initialBalance,
                _ibanGenerator,
                command.withdrawLimit,
                command.term);

            await _customerRepository.SaveAsync(customer);

            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(account.Id);
        }
    }
}
