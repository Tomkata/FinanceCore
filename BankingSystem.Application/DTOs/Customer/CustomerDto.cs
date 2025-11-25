namespace BankingSystem.Application.DTOs.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; init; }
        public string UserName  { get; init; }

        public string FirstName { get; init; }
        public string LastName { get; init; }

        public string PhoneNumber { get; init; }
        public string EGN { get; init; }

        public string Street { get; init; }
        public string City { get; init; }
        public string PostalCode { get; init; }
        public string Country { get; init; }    



    }
}
