using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Exceptions;

public class DepositAccount : Account
{
    public DepositTerm DepositTerm { get; private set; }
    public DateTime MaturityDate { get; private set; }

    public override AccountType AccountType => AccountType.Deposit;

    public DepositAccount(IBAN iban, Guid customerId, DepositTerm term)
        : base(iban, customerId)
    {
        DepositTerm = term ?? throw new InvalidDepositTerm();
        MaturityDate = term.CalculateMaturity(DateTime.UtcNow);
    }

    protected override void ValidateTypeSpecificWithdrawalRules(decimal amount)
    {
        if (DateTime.UtcNow < MaturityDate)
            throw new EarlyWithdrawalException(MaturityDate);
    }
}
