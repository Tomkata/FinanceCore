namespace BankingSystem.Application.UseCases.Accounts.WithdrawBankAccount
{
    using BankingSystem.Application.UseCases.Accounts.DepositBankAccount;
    using FluentValidation;
    public class WithdrawBankAccountValidator : AbstractValidator<WithdrawBankAccountCommand>
    {
        public WithdrawBankAccountValidator()
        {
            RuleFor(x => x.customerId)
              .NotEmpty();

            RuleFor(x => x.accountId)
                .NotEmpty();

            RuleFor(x => x.amount)
               .NotNull()
               .GreaterThan(0);
        }
    }
}
