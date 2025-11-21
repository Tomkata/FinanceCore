using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class DepositTermNullException : DomainException
    {
        public DepositTermNullException()
            : base("Cannot set maturity date without a deposit term.")
        {
        }
    }
}
