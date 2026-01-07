
namespace BankingSystem.Application.UseCases.Customers.UpdatePhoneNumber
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Domain.Interfaces;

    public class UpdatePhoneNumberHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdatePhoneNumberValidator _validator;

        public UpdatePhoneNumberHandler(
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork,
            UpdatePhoneNumberValidator validator)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(UpdatePhoneNumberCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
                return Result<Guid>.Failure(
                    string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.customerId);

            if (customer is null)
                return Result<Guid>.Failure("Customer not found");

            customer.UpdatePhoneNumber(command.phoneNumber);

            await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(command.customerId);
        }
    }
}