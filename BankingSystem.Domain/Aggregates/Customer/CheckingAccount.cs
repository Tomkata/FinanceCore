
namespace BankingSystem.Domain.Aggregates.Customer
{
    using BankingSystem.Domain.Enums;

    public class CheckingAccount : Account
    {
        private CheckingAccount()
            :base()
        {
        }

        public CheckingAccount(IBAN iban, Guid customerId) 
            : base(iban, customerId)
        {
        }

        public override AccountType AccountType => AccountType.Checking;


    }
}
