

namespace BankingSystem.Domain.Entities
{
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.Enums;
    using BankingSystem.Domain.ValueObjects;
    using BankingSystem.Domain.Exceptions;
    public class Account : BaseEntity
    {
        public Account(AccountType accountType, IBAN iBAN, Guid customerId, DepositTerm? depositTerm, int? withdrawLimits)
        {
            this.Transactions = new HashSet<Transaction>();
            this.AccountType = accountType;
            this.AccountStatus = AccountStatus.Active;
            this.CustomerId = customerId;
            this.IBAN = iBAN;
            this.Balance = 0;

            if (accountType == AccountType.Saving)
            {
                if (withdrawLimits == null)
                    throw new InvalidOperationException("Saving account must have withdraw limits.");

                this.WithdrawLimits = withdrawLimits;
                this.CurrentMonthWithdrawals = 0;
            }

            if (accountType == AccountType.Deposit)
            {
                if (depositTerm == null) throw new DepositTermNullException();

                this.DepositTerm = depositTerm;
                SetMaturityDate(DateTime.UtcNow);
            }
            else if (depositTerm != null)
            {
                throw new InvalidOperationException("Only Deposit accounts can have DepositTerm");
            }
        }
        private Account()
        { }
        public AccountType AccountType { get; private set; }
        public decimal Balance { get; private set; }
        public AccountStatus AccountStatus { get; private set; }
        public IBAN IBAN { get; private set; }
        public DateTime? MaturityDate { get; private set; } //only for Deposit Account
        public DepositTerm? DepositTerm { get; private set; }
        public int? WithdrawLimits { get; private set; }
        public int? CurrentMonthWithdrawals { get; private set; }
        public DateTime? LastWithdrawalDate { get; private set; }
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public virtual ICollection<Transaction> Transactions { get; private set; }


        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException(amount);
            }
            if (this.AccountStatus != AccountStatus.Active)
            {
                throw new AccountNotActiveException(this.Id, this.AccountStatus);

            }

            this.Balance += amount;
            this.UpdateTimeStamp();
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException(amount);
            }
            if (this.AccountStatus != AccountStatus.Active)
            {
                throw new AccountNotActiveException(this.Id, this.AccountStatus);
            }
            if (amount > this.Balance)
            {
                throw new InsufficientFundsException(amount, this.Balance);
            }

            if (this.AccountType == AccountType.Deposit && DateTime.UtcNow < this.MaturityDate)
            {
                throw new EarlyWithdrawalException(this.MaturityDate.Value);
            }
            if (IsSavingAccount()) //Cheks for saving account
            {
                if (LastWithdrawalDate.HasValue &&
                    (LastWithdrawalDate.Value.Month != DateTime.UtcNow.Month ||
                     LastWithdrawalDate.Value.Year != DateTime.UtcNow.Year))
                    CurrentMonthWithdrawals = 0;


                if (this.CurrentMonthWithdrawals >= this.WithdrawLimits)
                    throw new AccountWithdrawLimitException();

                this.CurrentMonthWithdrawals++;
                this.LastWithdrawalDate = DateTime.UtcNow;
            }


            this.Balance -= amount;
            this.UpdateTimeStamp();
        }

        private bool IsSavingAccount()
        {
            return this.AccountType == AccountType.Saving;
        }

        private void SetMaturityDate(DateTime startDate)
        {
            if (DepositTerm == null) throw new DepositTermNullException();

            this.MaturityDate = DepositTerm.CalculateMaturity(startDate);
        }

        public void Reactivate()
        {
            if (this.AccountStatus == AccountStatus.Closed) throw new InvalidOperationException("Cannot reactivate a closed account");

            this.AccountStatus = AccountStatus.Active;
            this.UpdateTimeStamp();
        }

        public void Freeze()
        {
            if (this.AccountStatus == AccountStatus.Closed) throw new InvalidOperationException("Cannot freeze a closed account");

            this.AccountStatus = AccountStatus.Blocked;
            this.UpdateTimeStamp();
        }


        public void Close()
        {
            if (this.AccountStatus == AccountStatus.Closed) throw new InvalidOperationException("This account is already closed");
            if (this.Balance > 0) throw new InvalidOperationException("Cannot close account with existing balance");

            this.AccountStatus = AccountStatus.Closed;
            this.UpdateTimeStamp();
        }


    }
}
