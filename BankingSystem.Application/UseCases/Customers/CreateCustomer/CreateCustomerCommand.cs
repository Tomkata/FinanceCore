using BankingSystem.Application.DTOs.Customer;

namespace BankingSystem.Application.UseCases.Customers.CreateCustomer
{   
    public class CreateCustomerCommand
    {
        /*
        Защо изобщо ни трябва Command, ако само wrap-ва DTO:
       1. Бъдеща гъвкавост
           public Guid CreatedByUserId { get; set; }  // ← Кой създава (за audit)
           public string IpAddress { get; set; }      // ← Откъде идва request-аs
       2. Validation контекст
           // Validator-ът валидира Command-а, не DTO-то директно

        */
        public CreateCustomerDto Data { get; set; }
    }
}
