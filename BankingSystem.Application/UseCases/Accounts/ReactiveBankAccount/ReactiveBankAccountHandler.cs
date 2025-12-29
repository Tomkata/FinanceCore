

using System.Net.NetworkInformation;

namespace BankingSystem.Application.UseCases.Accounts.ReactiveBankAccount
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Domain.Interfaces;
    public class ReactiveBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ReactiveBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public ReactiveBankAccountHandler(
            ICustomerRepository customerRepository,
            ReactiveBankAccountValidator validator,
            IUnitOfWork unitOfWork)
        {
            this._customerRepository = customerRepository;
            this._validator = validator;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(ReactiveBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.customerId);

            if (customer is null)
                return Result<Guid>.Failure("Customer not found");

            var account = customer.GetAccountById(command.accountId);

            account.Reactivate();


            await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(account.Id);

        }
    }
}
