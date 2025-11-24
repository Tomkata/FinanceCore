namespace BankingSystem.Domain.ValueObjects
{

    using Domain.Exceptions;

    public record DepositTerm
    {
        private DepositTerm()
        {}
        public int? Months { get; init; }

        public DepositTerm(int months)
        {
            if (months <= 0) throw new InvalidMonthsException(months);

            Months = months;
        }

        public DateTime CalculateMaturity(DateTime startDate)
        {
            if (Months == null)
                throw new InvalidOperationException("Months cannot be null");

            return startDate.AddMonths(Months.Value);
        }

    }
}
