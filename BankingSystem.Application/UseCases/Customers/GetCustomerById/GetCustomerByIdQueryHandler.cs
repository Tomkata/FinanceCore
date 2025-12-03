using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Domain.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.UseCases.Customers.GetCustomerById
{
    public class GetCustomerByIdQueryHandler
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
        {
            this._customerRepository = customerRepository;
        }

        public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery query) 
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(query.Id);

            if (existingCustomer is null)
                    return Result<CustomerDto>.Failure("Customer not found");

            var dto = existingCustomer.Adapt<CustomerDto>();

            return Result<CustomerDto>.Success(dto);

        }
    }
}
