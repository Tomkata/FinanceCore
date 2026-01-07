namespace BankingSystem.Application.UseCases.Customers.UpdateAddress
{
    public record UpdateAddressCommand(Guid customerId,string address,string city,int zip,string country);
}
