namespace BankingSystem.Domain.Exceptions
{
    public class InsufficientFundsException : DomainException
    {
        public decimal AttemptedAmount { get; }
        public decimal CurrentBalance { get; }

        public InsufficientFundsException(decimal amount,decimal balance)
        : base($"Insufficient funds. Attempted: {amount}, Available: {balance}.")
        {
            AttemptedAmount = amount;
            CurrentBalance = balance;
        }
    }
}
