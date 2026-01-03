using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class WithdrawLimitReachedException : DomainException
    {
        public WithdrawLimitReachedException()
            :base("Withdraw limit reached ")
        {
        }
    }
}
