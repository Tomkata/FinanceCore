namespace BankingSystem.Application.UseCases.Accounts.OpenBankAccount
{
    using BankingSystem.Domain.Entities;
    using BankingSystem.Domain.Enums;
    using FluentValidation;
    public class OpenBankAccountValidator : AbstractValidator<OpenBankAccountCommand>
    {
        public OpenBankAccountValidator()
        {
            RuleFor(x => x.customerId)
                .NotEmpty();

            RuleFor(x => x.initialBalance)
                .GreaterThanOrEqualTo(0);

           
            RuleFor(x => x.withdrawLimit)
                .NotNull()
                .GreaterThan(0)
                .When(x=>x.type==AccountType.Saving);

            RuleFor(x => x.withdrawLimit)
             .Null()
             .When(x => x.type != AccountType.Saving);

            RuleFor(x => x.term)
                .NotNull()
                .When(x => x.type == AccountType.Deposit);

            RuleFor(x => x.term)
                .Null()
                .When(x => x.type != AccountType.Deposit);

        }
    }
}
