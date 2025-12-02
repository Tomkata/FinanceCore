using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.UseCases.Accounts.DepositBankAccount
{
    public record DepositBankAccountCommand(Guid customerId, Guid accountId, decimal amount);
}
