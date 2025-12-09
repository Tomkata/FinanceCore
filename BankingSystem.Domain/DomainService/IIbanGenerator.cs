using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.DomainServices
{
    public interface IIbanGenerator
    {
        IBAN Generate(Guid Id);
    }
}
