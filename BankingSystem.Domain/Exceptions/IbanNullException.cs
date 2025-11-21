using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Exceptions
{
    [Serializable]
    public sealed class IbanException : DomainException
    {
        public IbanException(string message)
            : base(message) { }

        public IbanException(string message, Exception innerException)
            : base(message, innerException) { }

    }
}
