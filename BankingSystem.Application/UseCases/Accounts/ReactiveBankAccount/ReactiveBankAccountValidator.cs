

namespace BankingSystem.Application.UseCases.Accounts.ReactiveBankAccount
{
    using BankingSystem.Application.UseCases.Accounts.CloseBankAccount;
    using FluentValidation;
    public class ReactiveBankAccountValidator : AbstractValidator<ReactiveBankAccountCommand>
    {
        public ReactiveBankAccountValidator()
        {
            RuleFor(x => x.AccountId)
               .NotEmpty();

            RuleFor(x => x.CustomerId)
                .NotEmpty();
        }
    }
}
