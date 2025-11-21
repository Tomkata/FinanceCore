using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class InvalidPhoneNumberException : DomainException
    {

        public InvalidPhoneNumberException(string value)
            :base($"The phone number: {value} is not valid.")
        {
        }
    }
}
