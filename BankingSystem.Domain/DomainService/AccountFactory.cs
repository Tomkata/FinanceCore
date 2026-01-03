using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;

namespace BankingSystem.Domain.DomainServices
{
    public class AccountFactory : IAccountFactory
    {
        public Account Create(
            AccountType type,
            IBAN iban,
            Guid customerId,

            int? withdrawLimit = null,
            DepositTerm? depositTerm = null)
        {
            return type switch
            {
                AccountType.Checking => new CheckingAccount(iban, customerId),  

                AccountType.Saving => new SavingAccount(
                 iban,
                 customerId,
    withdrawLimit ?? throw new WithdrawLimitRequiredException()),

                AccountType.Deposit => new DepositAccount(
                    iban,
                    customerId,
                    depositTerm ?? throw new DepositTermRequiredException()
                ),

                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        public Account CreateSystemAccount(Guid id, IBAN iban, Guid customerId)
        {
            var account = new CheckingAccount(iban, customerId);
            return account;
        }
    }
}
