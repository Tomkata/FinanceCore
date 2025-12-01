using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class IdentityNullException : Exception
    {
        public IdentityNullException()
        :base("Identity cannot be null.")
        {
        }
    }
}
