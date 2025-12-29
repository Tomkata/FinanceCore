
namespace BankingSystem.Application.UseCases.Accounts.CloseBankAccount
{
    using FluentValidation;

    public class FreezeBankAccountValidator : AbstractValidator<FreezeBankAccountCommand>
    {
        public FreezeBankAccountValidator()
        {
            RuleFor(x => x.accountId)   
                .NotEmpty();

            RuleFor(x => x.customerId)
                .NotEmpty();
        }
    }
}
