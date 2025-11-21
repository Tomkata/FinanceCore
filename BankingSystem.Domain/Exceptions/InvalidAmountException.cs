namespace BankingSystem.Domain.Exceptions
{
    public class InvalidAmountException : DomainException
    {

        public decimal AttemptedAmount { get; }
  
        public InvalidAmountException(decimal amount) 
            : base($"Invalid amount: {amount}. Must be greater than zero.") 
        {
            AttemptedAmount = amount;
        }   
        
    }   
}
