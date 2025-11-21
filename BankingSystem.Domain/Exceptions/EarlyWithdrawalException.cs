namespace BankingSystem.Domain.Exceptions
{
    public class EarlyWithdrawalException : DomainException
    {
        public DateTime MaturityDate { get; }

        public EarlyWithdrawalException(DateTime maturityDate)
            :base($"Cannot withdraw before maturity date: {maturityDate:yyyy-MM-dd}."
)
        {
            MaturityDate = maturityDate;
        }
    }
}
