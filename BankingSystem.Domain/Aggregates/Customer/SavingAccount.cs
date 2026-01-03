using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Exceptions;

namespace BankingSystem.Domain.Aggregates.Customer
{
    public class SavingAccount : Account
    {
        public int WithdrawLimits { get; private set; }
        public int CurrentMonthWithdrawals { get; private set; }
        public DateTime? LastWithdrawalDate { get; private set; }

        public override AccountType AccountType => AccountType.Saving;

        private SavingAccount() : base() { }

        public SavingAccount(IBAN iban, Guid customerId, int withdrawLimit)
            : base(iban, customerId)
        {
            if (withdrawLimit <= 0)
                throw new AccountWithdrawInvalidParameter(withdrawLimit);

            WithdrawLimits = withdrawLimit;
        }

        protected override void ValidateTypeSpecificWithdrawalRules(decimal amount)
        {
            if (LastWithdrawalDate?.Month != DateTime.UtcNow.Month)
                CurrentMonthWithdrawals = 0;

            if (CurrentMonthWithdrawals >= WithdrawLimits)
                throw new WithdrawLimitReachedException();
        }

        protected override void OnWithdrawalCompleted(decimal amount)
        {
            CurrentMonthWithdrawals++;
            LastWithdrawalDate = DateTime.UtcNow;
        }
    }
}
