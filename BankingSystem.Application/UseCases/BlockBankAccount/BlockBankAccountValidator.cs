using FluentValidation;

namespace BankingSystem.Application.UseCases.BlockBankAccount
{
    public class BlockBankAccountValidator : AbstractValidator<BlockBankAccountCommand>
    {
        public BlockBankAccountValidator()
        {
            RuleFor(x => x.AccountId)
               .NotEmpty();

            RuleFor(x => x.CustomerId)
                .NotEmpty();
        }
    }
}
