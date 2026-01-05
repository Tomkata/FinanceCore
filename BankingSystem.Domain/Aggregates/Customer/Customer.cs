

namespace BankingSystem.Domain.Aggregates.Customer
{
    using BankingSystem.Domain.Aggregates.Customer.Events;
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.DomainService;
    using BankingSystem.Domain.DomainServices;
    using BankingSystem.Domain.Enums;
    using BankingSystem.Domain.Enums.Account;
    using BankingSystem.Domain.Enums.Customer;
    using BankingSystem.Domain.Exceptions;
    using BankingSystem.Domain.ValueObjects;
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
        }

        public string UserName { get;private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Address Address { get;private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public EGN EGN { get; private set; }
        public CustomerStatus Status { get; private set; }

        public virtual ICollection<Account> Accounts { get; private set; } = new HashSet<Account>();

        public static Customer CreateSystemCustomer(
    Guid id,
    string userName,
    string firstName,
    string lastName,
    PhoneNumber phoneNumber,
    Address address,
    EGN egn)
        {
            var customer = new Customer(
                userName,
                firstName,
                lastName,
                phoneNumber,
                address,
                egn
            );

            customer.Id = id;
            return customer;
        }


        public Account OpenAccount(
            AccountType type,
            decimal initialBalance,
            IIbanGenerator ibanGenerator,
            IAccountFactory accountFactory,
            int? withdrawLimit = null,
            DepositTerm? depositTerm = null)
        {
            if (this.Status != CustomerStatus.Active)
                throw new CannotOpenAccountForInactiveCustomerException();

            if(initialBalance<0)
                throw new InvalidAmountException(initialBalance);

            if (type == AccountType.Deposit && this.Accounts.Any(a => a.AccountType == AccountType.Deposit))
                throw new CannotHaveMultipleDepositAccountsException();

            var iban = ibanGenerator.Generate(this.Id);


            var account = accountFactory.Create(
                 type,
                 iban,
                 this.Id,
                 withdrawLimit,
                 depositTerm
             );

            if (initialBalance > 0)
            {
                account.Deposit(initialBalance);
            }

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

        public void Transfer(Guid fromAccountId, Guid toAccountId, decimal amount)
        {

            var from = GetAccountById(fromAccountId);
            var to = GetAccountById(toAccountId);

            from.Withdraw(amount);
            to.Deposit(amount);

            AddDomainEvent(new TransferInitiatedEvent(
                this.Id,
                fromAccountId,
                toAccountId,
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
                throw new AccountNotFoundException(accountId);

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