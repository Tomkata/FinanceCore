using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class DepositTermRequiredException : DomainException
    {
        public DepositTermRequiredException()
            :base("DepositTermRequiredException")
        {
        }
    }
}
