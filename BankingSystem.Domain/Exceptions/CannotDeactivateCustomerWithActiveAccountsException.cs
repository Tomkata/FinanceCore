using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class CannotDeactivateCustomerWithActiveAccountsException : DomainException
    {
        public CannotDeactivateCustomerWithActiveAccountsException()
           :base("Customer has active accounts and cannot be deactivated.")
        {
            
        }
    }
}
    