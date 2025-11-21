using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class AccountWithdrawLimitException : DomainException
    {
        public AccountWithdrawLimitException()
            : base($"Your account withdrawal limit has been reached.")
        {
        }

    }
}
