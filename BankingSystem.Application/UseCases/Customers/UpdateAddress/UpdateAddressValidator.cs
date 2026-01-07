using BankingSystem.Application.UseCases.Customers.OpenBankAccount;
using FluentValidation;

namespace BankingSystem.Application.UseCases.Customers.UpdateAddress
{
    public class UpdateAddressValidator : AbstractValidator<UpdateAddressCommand>
    {
        public UpdateAddressValidator()
        {
            RuleFor(x => x.customerId)
              .NotEmpty()
              .WithMessage("Customer ID is required");

            RuleFor(x => x.address)
                .NotEmpty()
                .WithMessage("Address is required")
                .MaximumLength(200)
                .WithMessage("Address cannot exceed 200 characters");

            RuleFor(x => x.city)
                .NotEmpty()
                .WithMessage("City is required")
                .MaximumLength(100)
                .WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.zip)
                .GreaterThan(0)
                .WithMessage("ZIP code must be greater than 0")
                .LessThan(100000)
                .WithMessage("ZIP code must be less than 100000");

            RuleFor(x => x.country)
                .NotEmpty()
                .WithMessage("Country is required")
                .MaximumLength(100)
                .WithMessage("Country cannot exceed 100 characters");

        }
    }
}
