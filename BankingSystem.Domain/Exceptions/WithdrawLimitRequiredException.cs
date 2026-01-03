namespace BankingSystem.Domain.Exceptions
{
    public class WithdrawLimitRequiredException : DomainException
    {
        public WithdrawLimitRequiredException()
            : base("Saving accounts must specify a withdraw limit.") { }
    }
}
