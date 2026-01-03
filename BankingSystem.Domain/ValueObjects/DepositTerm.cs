using BankingSystem.Domain.Exceptions;

public record DepositTerm
{
    public int Months { get; init; }

    private DepositTerm() { }

    public DepositTerm(int months)
    {
        if (months <= 0)
            throw new InvalidMonthsException(months);

        Months = months;
    }

    public DateTime CalculateMaturity(DateTime startDate)
        => startDate.AddMonths(Months);
}
