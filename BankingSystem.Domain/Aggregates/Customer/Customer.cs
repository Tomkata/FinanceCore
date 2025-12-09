using BankingSystem.Domain.Aggregates.Customer.Events;
using BankingSystem.Domain.Common;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Account;
using BankingSystem.Domain.Enums.Customer;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;

namespace BankingSystem.Domain.Aggregates.Customer
{
    public class Customer : AggregateRoot
    {
        private Customer() {  }
        public Customer(string userName, 
            string firstName,   
            string lastName,
            PhoneNumber phoneNumber,
            Address address,
            EGN eGN)
        {
            this.UserName = userName;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.PhoneNumber = phoneNumber;
            this.Address = address;
            this.EGN = eGN ?? throw new ArgumentNullException(nameof(eGN));
            this.Accounts = new HashSet<Account>();
        }

        public string UserName { get;private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Address Address { get;private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public EGN EGN { get; private set; }
        public CustomerStatus Status { get; private set; }

        public virtual ICollection<Account> Accounts { get; private set; }


        public Account OpenAccount(
            AccountType type,
            decimal initialBalance,
            IIbanGenerator ibanGenerator,
            int? withdrawLimit = null,
            DepositTerm? depositTerm = null)
        {
            if (this.Status != CustomerStatus.Active)
                throw new CannotOpenAccountForInactiveCustomerException();

            if(initialBalance<0)
                throw new InvalidAmountException(initialBalance);

                if(type == AccountType.Deposit && this.Accounts.Any(a => a.AccountType == AccountType.Deposit))
    throw new CannotHaveMultipleDepositAccountsException();

            var iban = ibanGenerator.Generate(this.Id);

            Account account = type switch
            {
                AccountType.Checking => Account.CreateRegular(iban, this.Id),

                AccountType.Saving => withdrawLimit is null
                    ? throw new InvalidOperationException("Withdraw limit required for Saving account.")
                    : Account.CreateSaving(iban, this.Id, withdrawLimit.Value),

                AccountType.Deposit => depositTerm is null
                    ? throw new InvalidOperationException("Withdraw limit required")
                    : Account.CreateDeposit(iban, this.Id, depositTerm),

                _ => throw new InvalidOperationException("Unknown account type")
            };

            if (initialBalance > 0)
                account.Deposit(initialBalance);

            this.Accounts.Add(account);

            return account;
        }

        public void Deposit(Guid accountId, decimal amount)
        {
            var account = GetAccountById(accountId);

            account.Deposit(amount);
            
            AddDomainEvent(new AccountCreditedEvent(
                this.Id,
                accountId,
                amount
            ));
        }


        public void Withdraw(Guid accountId, decimal amount)
        {
            var account = GetAccountById(accountId);

            account.Withdraw(amount);

            AddDomainEvent(new AccountDebitedEvent(
                this.Id,
                accountId,
                amount
            ));
        }


        public Account GetAccountById(Guid accountId)
        {
            var account = this.Accounts.SingleOrDefault(x => x.Id == accountId);
            if (account == null)
                throw new AccountNotFoundException();

            return account;
        }

        public void UpdateAddress(string address, string city, int zip, string country)
        {
            this.Address = new Address(address,city,zip,country);
        }

        public void UpdatePhoneNumber(string phoneNumber)
        {
            this.PhoneNumber = new PhoneNumber(phoneNumber);
            this.UpdateTimeStamp();
        }

        public void Deactivate()
        {
            if (this.Accounts.Any(x=>x.AccountStatus==AccountStatus.Active))
                throw new CannotDeactivateCustomerWithActiveAccountsException();

            this.Status = CustomerStatus.Inactive;
           
        }
    }
}