namespace BankingSystem.Domain.Exceptions
{
    public class InvalidMonthsException : DomainException
    {
        public int AttemptedMonths { get; }

        public InvalidMonthsException(int attemptedMonths) 
            : base($"Invalid months: {attemptedMonths}. Must be greater than zero.")
        {
            AttemptedMonths = attemptedMonths;
        }
    }
}
