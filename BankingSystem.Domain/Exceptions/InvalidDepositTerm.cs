using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class InvalidDepositTerm : DomainException
    {
        public InvalidDepositTerm()
            : base($"Invalid deposit term.")
        {

        }
    }
}
