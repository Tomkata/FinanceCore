using BankingSystem.Domain.Enums;
using BankingSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.UseCases.Accounts.OpenBankAccount
{
    public record OpenBankAccountCommand(
        AccountType type,
        Guid customerId,
        decimal initialBalance,
        int? withdrawLimit,
        DepositTerm? term
        );
}
