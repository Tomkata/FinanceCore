using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Application.UseCases.Accounts.CloseBankAccount
{
    public class CloseBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly CloseBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public CloseBankAccountHandler(ICustomerRepository customerRepository, 
            CloseBankAccountValidator validator,
            IUnitOfWork unitOfWork)
        {
            this._customerRepository = customerRepository;
            this._validator = validator;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CloseBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.customerId);

            if(customer is null)
                return Result<Guid>.Failure("Customer not found");

            var account = customer.GetAccountById(command.accountId); //not async !!
            account.Close();

           await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(account.Id);

        }
    }
}
