namespace BankingSystem.Application.UseCases.Customers.UpdatePhoneNumber
{
    public record UpdatePhoneNumberCommand(Guid customerId, string phoneNumber);
}
