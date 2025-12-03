

namespace BankingSystem.Application.UseCases.Customers.CreateCustomer
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Application.DTOs.Customer;
    using BankingSystem.Domain.Entities;
    using BankingSystem.Domain.Exceptions;
    using BankingSystem.Domain.Interfaces;
    using Mapster;
    public class CreateCustomerHandler
    {
        //inject
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateCustomerValidator _validator;

        public CreateCustomerHandler(ICustomerRepository customerRepository, 
            IUnitOfWork unitOfWork,
            CreateCustomerValidator validator)
        {
            this._customerRepository = customerRepository;
            this._unitOfWork = unitOfWork;
            this._validator = validator;
        }

        public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand command)
        {

            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
                return Result<CustomerDto>.Failure(validationResult.ToString());
                
            var existingCustomer = await _customerRepository.FindByEgnAsync(command.Data.EGN);

            if (existingCustomer is not null)
                throw new CustomerWithExistingEgnException( );

            //map
            var customer = command.Adapt<Customer>();

            await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return Result<CustomerDto>.Success(customer.Adapt<CustomerDto>());

        }


    }
}
