using BankingSystem.Domain.Common;
using BankingSystem.Domain.ValueObjects;

namespace BankingSystem.Domain.Entities
{
    public class Customer : BaseEntity
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
       

        public virtual ICollection<Account> Accounts { get; private set; }

        public void UpdateAddress(string address, string city, int zip, string country)
        {
            this.Address = new Address(address,city,zip,country);
        }

        public void UpdatePhoneNumber(string phoneNumber)
        {
            this.PhoneNumber = new PhoneNumber(phoneNumber);
            this.UpdateTimeStamp();
        }
    }
}