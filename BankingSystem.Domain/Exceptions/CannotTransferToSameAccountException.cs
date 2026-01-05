namespace BankingSystem.Domain.Exceptions
{
    public class CannotTransferToSameAccountException : DomainException
    {
        public CannotTransferToSameAccountException()
            : base("Cannot transfer to the same account.")
        {
        }
    }
}
