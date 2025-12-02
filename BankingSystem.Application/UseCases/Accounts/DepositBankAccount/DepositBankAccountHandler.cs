using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.UseCases.Accounts.CloseBankAccount;
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

        public DepositBankAccountHandler(ICustomerRepository customerRepository,
            DepositBankAccountValidator validator,
            IUnitOfWork unitOfWork)
        {
            this._customerRepository = customerRepository;
            this._validator = validator;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DepositBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(String.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));




        }
    }
}
