

namespace BankingSystem.Application.UseCases.Accounts.WithdrawBankAccount
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Domain.Interfaces;
    public class WithdrawBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly WithdrawBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public WithdrawBankAccountHandler(
            ICustomerRepository customerRepository,
            WithdrawBankAccountValidator validator,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(WithdrawBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(String.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.customerId);

            if (customer is null)
                return Result<Guid>.Failure("Customer not found");

            customer.Withdraw(command.accountId, command.amount);

            await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync(); 

            return Result<Guid>.Success(command.accountId);
        }
    }

}
}
