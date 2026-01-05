namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    using FluentValidation;
    public class TransferBankAccountValidator : AbstractValidator<TransferToBankAccountCommand>
    {
        public TransferBankAccountValidator()
        {
            RuleFor(x => x.toAccountid)
    .NotEqual(x => x.fromAccountId)
    .WithMessage("Cannot transfer to the same account");



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
