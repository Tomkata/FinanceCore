namespace BankingSystem.Domain.Exceptions;

public class InvalidAccountStateException : DomainException
{
    public InvalidAccountStateException(string message) : base(message) { }
}
