using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    public class CustomerWithExistingEgnException : Exception
    {

    
        public CustomerWithExistingEgnException() 
            : base("Customer with this EGN already exists.")
        {
        }
    }
}
    