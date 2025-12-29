using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.UseCases.Accounts.CloseBankAccount;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Application.UseCases.BlockBankAccount
{
    public class BlockBankAccountHandler
    {

        private readonly ICustomerRepository _customerRepository;
        private readonly BlockBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public BlockBankAccountHandler(
            ICustomerRepository customerRepository, 
            BlockBankAccountValidator validator,
            IUnitOfWork unitOfWork)
        {
            this._customerRepository = customerRepository;
            this._validator = validator;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(BlockBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));


            var customer = await _customerRepository.GetByIdAsync(command.customerId);
            if (customer is null)
                return Result<Guid>.Failure("Customer not found");

            var account = customer.GetAccountById(command.accountId); //not async !!
            account.Freeze();

            await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(account.Id);
        } 
    }
}
