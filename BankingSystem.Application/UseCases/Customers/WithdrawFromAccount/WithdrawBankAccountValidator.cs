namespace BankingSystem.Application.UseCases.Customers.WithdrawFromAccount
{
    using BankingSystem.Application.UseCases.Customers.DepositToAccount;
    using FluentValidation;
    public class WithdrawBankAccountValidator : AbstractValidator<WithdrawBankAccountCommand>
    {
        public WithdrawBankAccountValidator()
        {
            RuleFor(x => x.CustomerId)
              .NotEmpty();

            RuleFor(x => x.AccountId)
                .NotEmpty();

            RuleFor(x => x.Amount)
               .NotNull()
               .GreaterThan(0);
        }
    }
}
