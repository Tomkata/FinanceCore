using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure.Services
{
    public class IbanGenerator : IIbanGenerator
    {
        public IBAN Generate(Guid Id)
        {
            var raw = "BG80" + "BANK" + Id.ToString().Replace("-", "").Substring(0, 14);
            return IBAN.Create(raw);

            // simple example, replace later with real logic
            }
        }
    }
