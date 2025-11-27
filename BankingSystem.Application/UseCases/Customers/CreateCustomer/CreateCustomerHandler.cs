using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.ValueObjects;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Application.UseCases.Customers.CreateCustomer
{
    public class CreateCustomerHandler
    {
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

            //validation
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ",validationResult.Errors.Select(x=>x.ErrorMessage));
                return Result<CustomerDto>.Failure(errors);
            }

            try
            {
                var egn = EGN.Create(command.Data.EGN);
                var phoneNumber = new PhoneNumber(command.Data.PhoneNumber);
                if (!int.TryParse(command.Data.PostalCode, out int postalCode))
                {
                    return Result<CustomerDto>.Failure("Invalid postal code format");
                }
                var address = new Address(
                    command.Data.Street,
                    command.Data.City,
                   postalCode,
                    command.Data.Country
                    );

                var customer = new Customer(
                    command.Data.UserName,
                    command.Data.FirstName,
                    command.Data.LastName,
                    phoneNumber,
                    address,
                    egn
                    );

               await _customerRepository.SaveAsync(customer);
                await _unitOfWork.SaveChangesAsync();


                var dto = new CustomerDto
                {
                    Id = customer.Id,
                    UserName = customer.UserName,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    PhoneNumber = customer.PhoneNumber.Value,
                    EGN = customer.EGN.Value,
                    City = customer.Address.City,
                    Country = customer.Address.Country,
                    PostalCode = postalCode.ToString(),
                    Street = customer.Address.CityAddress
                };

                return  Result<CustomerDto>.Success(dto);

            }
            catch (DomainException ex)
            {

                return Result<CustomerDto>.Failure(ex.Message);
            }
            catch (ArgumentException ex)  
            {
                return Result<CustomerDto>.Failure(ex.Message);
            }
            catch (Exception ex)  
            {
                return Result<CustomerDto>.Failure($"An error occurred: {ex.Message}");
            }
        }


    }
}
