namespace BankingSystem.Application.UseCases.Customers.DepositToAccount
{
    using FluentValidation;

    public class DepositBankAccountValidator : AbstractValidator<DepositBankAccountCommand>
    {
        public DepositBankAccountValidator()
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
