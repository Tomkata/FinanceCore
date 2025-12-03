namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    using FluentValidation;
    public class TransferBankAccountValidator : AbstractValidator<TransferBankAccountCommand>
    {
        public TransferBankAccountValidator()
        {
            RuleFor(x => x.customerId)
              .NotEmpty();

            RuleFor(x => x.fromAccountId)
                .NotEmpty();

             RuleFor(x => x.toAccountid)
                .NotEmpty();

            RuleFor(x => x.amount)
               .NotNull()
               .GreaterThan(0);

        }
    }
}
