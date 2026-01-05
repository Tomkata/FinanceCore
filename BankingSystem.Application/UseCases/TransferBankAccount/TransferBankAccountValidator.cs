namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    using FluentValidation;
    public class TransferBankAccountValidator : AbstractValidator<TransferBankAccountCommand>
    {
        public TransferBankAccountValidator()
        {
            RuleFor(x => x.SenderCustomerId)
                .NotEmpty()
                .WithMessage("Sender customer ID is required");

            RuleFor(x => x.ReceiverCustomerId)
                .NotEmpty()
                .WithMessage("Receiver customer ID is required");

            RuleFor(x => x.FromAccountId)
                .NotEmpty()
                .WithMessage("From account ID is required");

            RuleFor(x => x.ToAccountId)
                .NotEmpty()
                .WithMessage("To account ID is required");

            RuleFor(x => x.Amount)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0");

            // Business rule: cannot transfer to the same account
            RuleFor(x => x.FromAccountId)
                .NotEqual(x => x.ToAccountId)
                .WithMessage("Cannot transfer to the same account");
        }
    }
}
