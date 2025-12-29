

namespace BankingSystem.Application.UseCases.Accounts.ReactiveBankAccount
{
    using BankingSystem.Application.UseCases.Accounts.CloseBankAccount;
    using FluentValidation;
    public class ReactiveBankAccountValidator : AbstractValidator<ReactiveBankAccountCommand>
    {
        public ReactiveBankAccountValidator()
        {
            RuleFor(x => x.accountId)
               .NotEmpty();

            RuleFor(x => x.customerId)
                .NotEmpty();
        }
    }
}
