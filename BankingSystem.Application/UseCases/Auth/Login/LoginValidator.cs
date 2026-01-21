using FluentValidation;

namespace BankingSystem.Application.UseCases.Auth.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Data.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Data.Password)
                .NotEmpty()
                .WithMessage("Password is required");
        }
    }
}
