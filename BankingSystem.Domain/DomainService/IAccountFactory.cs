
namespace BankingSystem.Domain.DomainService
{
    using BankingSystem.Domain.Aggregates.Customer;
    using BankingSystem.Domain.DomainServices;
    using BankingSystem.Domain.Enums;
    using BankingSystem.Domain.ValueObjects;

    public  interface IAccountFactory
    {
            Account Create( 
          AccountType type,
          IBAN iban,
          Guid customerId,
          int? withdrawLimit = null,
          DepositTerm? depositTerm = null
      );

        public  Account CreateSystemAccount(Guid id, IBAN iban, Guid customerId);
    }
}
