
namespace BankingSystem.Domain.Exceptions
{
    using BankingSystem.Domain.Enums;

    public class AccountNotActiveException : DomainException
    {
        public Guid AccountId { get;  } 
        public AccountStatus AccountStatus { get;}

        public AccountNotActiveException(Guid accountId,AccountStatus accountStatus)
            :base($"Account {accountId} has invalid status: {accountStatus}.")
        {
            AccountId = accountId;
            AccountStatus = accountStatus;
        }
    }
}
