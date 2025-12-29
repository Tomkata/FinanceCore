namespace BankingSystem.Application.UseCases.Customers.OpenBankAccount
{
    using BankingSystem.Domain.Enums;
    using BankingSystem.Domain.ValueObjects;

    public record   OpenBankAccountCommand(
        AccountType type,
        Guid customerId,
        decimal initialBalance,
        int? withdrawLimit,
        DepositTerm? term
        );
}
            