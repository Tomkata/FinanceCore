using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class CannotTransferToSameAccountException :  DomainException
    {
        public CannotTransferToSameAccountException()
            :base("Cannot transfer between same accounts")
        {
        }
    }
}
