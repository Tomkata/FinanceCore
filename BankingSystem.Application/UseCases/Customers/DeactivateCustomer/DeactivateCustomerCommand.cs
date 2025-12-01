using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.UseCases.Customers.DeactivateCustomer
{
    public record DeactivateCustomerCommand(Guid Id);
}
