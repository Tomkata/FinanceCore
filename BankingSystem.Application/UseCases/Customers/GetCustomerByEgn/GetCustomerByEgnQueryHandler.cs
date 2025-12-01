

namespace BankingSystem.Application.UseCases.Customers.GetCustomerByEgn
{
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Application.DTOs.Customer;
    using BankingSystem.Domain.Interfaces;
    using Mapster;
        
    public  class GetCustomerByEgnQueryHandler
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByEgnQueryHandler(ICustomerRepository customerRepository)
        {
            this._customerRepository = customerRepository;
        }

        public async Task<Result<CustomerDto>> Handle(GetCustomerByEgnQuery query)
        {
            var existingCustomer = await _customerRepository.FindByEgnAsync(query.Egn);

            if (existingCustomer is null)
                return Result<CustomerDto>.Failure("Customer not found");

            var customer = existingCustomer.Adapt<CustomerDto>();

            return Result<CustomerDto>.Success(customer);

        }   
    }
}
