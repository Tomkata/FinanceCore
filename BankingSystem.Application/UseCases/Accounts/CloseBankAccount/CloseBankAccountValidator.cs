
namespace BankingSystem.Application.UseCases.Accounts.CloseBankAccount
{
    using FluentValidation;

    public class CloseBankAccountValidator : AbstractValidator<CloseBankAccountCommand>
    {
        public CloseBankAccountValidator()
        {
            RuleFor(x => x.accountId)   
                .NotEmpty();

            RuleFor(x => x.customerId)
                .NotEmpty();
        }
    }
}
