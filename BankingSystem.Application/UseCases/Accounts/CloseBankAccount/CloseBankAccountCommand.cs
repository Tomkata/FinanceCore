using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.UseCases.Accounts.CloseBankAccount
{
    public record CloseBankAccountCommand(Guid customerId, Guid accountId);
}
