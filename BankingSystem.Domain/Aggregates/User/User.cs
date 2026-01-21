
namespace BankingSystem.Domain.Aggregates.User
{
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.Aggregates.Customer;

    public class User : BaseEntity
    {
        private User()
        {
        }

        public User(string email, string firstName, string lastName)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }

        public string Email { get; private set; }
        public string FirstName  { get; private set; }
        public string LastName { get; private set; }

        public Guid? CustomerId { get; private set; }
        public virtual Customer Customer { get; private set; }

        public void LinkToCustomer(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
