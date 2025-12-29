using FluentValidation;

namespace BankingSystem.Application.UseCases.BlockBankAccount
{
    public class BlockBankAccountValidator : AbstractValidator<BlockBankAccountCommand>
    {
        public BlockBankAccountValidator()
        {
            RuleFor(x => x.accountId)
               .NotEmpty();

            RuleFor(x => x.customerId)
                .NotEmpty();
        }
    }
}
