
namespace BankingSystem.Application.UseCases.Customers.CreateCustomer
{
    using FluentValidation;

    public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerValidator()
        {
            RuleFor(x => x.Data.FirstName)
                .NotEmpty()
               .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Data.LastName)
               .NotEmpty()
               .MinimumLength(3)
               .MaximumLength(50);

            RuleFor(x => x.Data.UserName)
               .NotEmpty()
               .MinimumLength(3)
               .MaximumLength(50);

            RuleFor(x => x.Data.PhoneNumber)
                .NotEmpty()
                .Matches("^\\+?[0-9]{10,15}$");

            RuleFor(x => x.Data.EGN)
                .NotEmpty()
                .Matches("^[0-9]{10}$");

            RuleFor(x => x.Data.PostalCode)
                .NotEmpty()
                .Matches("\\+?[0-9]{4,10}");

            RuleFor(x => x.Data.Street)
               .NotEmpty()
               .MaximumLength(200);

            RuleFor(x => x.Data.City)
             .NotEmpty()
             .MaximumLength(100);

            RuleFor(x => x.Data.Country)
             .NotEmpty()
             .MaximumLength(100);

        }
    }
}
