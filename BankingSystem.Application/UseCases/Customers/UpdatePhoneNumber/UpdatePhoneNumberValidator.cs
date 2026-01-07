
namespace BankingSystem.Application.UseCases.Customers.UpdatePhoneNumber
{
using FluentValidation;
    public class UpdatePhoneNumberValidator : AbstractValidator<UpdatePhoneNumberCommand>
    {
        public UpdatePhoneNumberValidator()
        {
            RuleFor(x => x.phoneNumber)
                .NotEmpty();

            RuleFor(x => x.phoneNumber)
               .NotEmpty()
               .WithMessage("Phone number is required")
               .MinimumLength(10)
               .WithMessage("Phone number must be at least 10 characters")
               .MaximumLength(20)
               .WithMessage("Phone number cannot exceed 20 characters");
        }
    }
}
