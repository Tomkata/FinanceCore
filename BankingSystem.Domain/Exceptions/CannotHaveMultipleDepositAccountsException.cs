using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class CannotHaveMultipleDepositAccountsException : DomainException
    {
        public CannotHaveMultipleDepositAccountsException()
        :base("Customer cannot have multiple deposit accounts")
        {
            
        }
    }
}
