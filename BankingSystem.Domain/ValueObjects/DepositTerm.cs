namespace BankingSystem.Domain.ValueObjects
{

    using Domain.Exceptions;

    public record DepositTerm
    {
        public int Months { get;}

        public DepositTerm(int months)
        {
            if (months <= 0) throw new InvalidMonthsException(months);

            Months = months;
        }

        public DateTime CalculateMaturity(DateTime startDate)
        {
            return startDate.AddMonths(Months);
        }
    }
}
