namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    using FluentValidation;
    public class TransferBankAccountValidator : AbstractValidator<TransferBankAccountCommand>
    {
        public TransferBankAccountValidator()
        {
            RuleFor(x => x.receiverCustomerId)
    .NotEmpty().WithMessage("Receiver customer ID is required");

            RuleFor(x => x.senderCustomerId)
                .NotEqual(x => x.receiverCustomerId)
                .WithMessage("Cannot transfer to same customer (use different endpoint for same-customer transfers)");

            RuleFor(x => x.fromAccountId)
                .NotEqual(x => x.toAccountId)
                .WithMessage("Cannot transfer to the same account");


            RuleFor(x => x.toAccountId)
                .NotEmpty();

            RuleFor(x => x.fromAccountId)
               .NotEmpty();

            RuleFor(x => x.amount)
               .NotNull()
               .GreaterThan(0);

        }
    }
}
