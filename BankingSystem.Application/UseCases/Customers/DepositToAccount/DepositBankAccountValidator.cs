namespace BankingSystem.Application.UseCases.Customers.DepositToAccount
{
    using FluentValidation;

    public class DepositBankAccountValidator : AbstractValidator<DepositBankAccountCommand>
    {
        public DepositBankAccountValidator()
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
