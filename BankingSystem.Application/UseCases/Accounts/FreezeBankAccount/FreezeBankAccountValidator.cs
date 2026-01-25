
namespace BankingSystem.Application.UseCases.Accounts.CloseBankAccount
{
    using FluentValidation;

    public class FreezeBankAccountValidator : AbstractValidator<FreezeBankAccountCommand>
    {
        public FreezeBankAccountValidator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty();

            RuleFor(x => x.CustomerId)
                .NotEmpty();
        }
    }
}
