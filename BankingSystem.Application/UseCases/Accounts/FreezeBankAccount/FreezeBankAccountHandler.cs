
namespace BankingSystem.Application.UseCases.Accounts.CloseBankAccount
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Domain.Interfaces;

    public class FreezeBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly FreezeBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public FreezeBankAccountHandler(ICustomerRepository customerRepository, 
            FreezeBankAccountValidator validator,
            IUnitOfWork unitOfWork)
        {
            this._customerRepository = customerRepository;
            this._validator = validator;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(FreezeBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.CustomerId);

            if(customer is null)
                return Result<Guid>.Failure("Customer not found");

            var account = customer.GetAccountById(command.AccountId); 
            account.Freeze();
                
           await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(account.Id);

        }
    }
}
