using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Application.UseCases.Customers.UpdateAddress
{
    public class UpdateAddressHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UpdateAddressValidator _validator;

        public UpdateAddressHandler(ICustomerRepository customerRepository, 
                                    IUnitOfWork unitOfWork,
                                    UpdateAddressValidator validator)
        {
            this._customerRepository = customerRepository;
            this._unitOfWork = unitOfWork;
            this._validator = validator;
        }


        public async Task<Result<Guid>> Handle(UpdateAddressCommand command)
        {
            var  validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
                return Result<Guid>.Failure(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.customerId);

            if (customer is null)
                return Result<Guid>.Failure("Customer not found");

            customer.UpdateAddress(command.address,command.city,command.zip,command.country);

            await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(command.customerId);
        }
    }
}
