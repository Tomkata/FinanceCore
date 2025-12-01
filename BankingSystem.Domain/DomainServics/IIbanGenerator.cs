using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.DomainServics
{
    public interface IIbanGenerator
    {
        IBAN Generate(Guid Id);
    }
}
