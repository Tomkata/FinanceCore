using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.UseCases.Customers.DeactivateCustomer
{
    public class DeactivateCustomerCommandHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private IUnitOfWork _unitOfWork;

        public DeactivateCustomerCommandHandler(ICustomerRepository customerRepository,
                                                 IUnitOfWork unitOfWork)
        {
            this._customerRepository = customerRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeactivateCustomerCommand command)
        {
            var customer = await _customerRepository.GetByIdAsync(command.Id);

            if (customer is null)
                return Result<bool>.Failure("Customer not found.");

            customer.Deactivate();

            await _customerRepository.SaveAsync(customer);
            await _unitOfWork.SaveChangesAsync();


            return Result<bool>.Success(true);
        }
    }
}
