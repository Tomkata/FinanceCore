using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class CannotOpenAccountForInactiveCustomerException : Exception
    {
        public CannotOpenAccountForInactiveCustomerException()
            :base("Cannot open account for inactive customer.")
        {
        }
    }
}
