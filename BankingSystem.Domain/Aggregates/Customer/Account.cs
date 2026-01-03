using BankingSystem.Domain.Common;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Account;
using BankingSystem.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystem.Domain.Aggregates.Customer
{
    public abstract class Account : BaseEntity
    {
        public IBAN IBAN { get; protected set; }
        public Guid CustomerId { get; protected set; }
        public decimal Balance { get; protected set; }
        public AccountStatus AccountStatus { get; protected set; } = AccountStatus.Active;
        [NotMapped]
        public abstract AccountType AccountType { get; }
        public Customer Customer { get; protected set; }
        public virtual ICollection<BankingSystem.Domain.Aggregates.Transaction.Transaction> Transactions { get; protected set; } 
            = new HashSet<BankingSystem.Domain.Aggregates.Transaction.Transaction>();
        protected Account(IBAN iban, Guid customerId)
        {
            IBAN = iban ?? throw new ArgumentNullException(nameof(iban));
            CustomerId = customerId;
        }

        protected Account()
        {
        }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new InvalidAmountException(amount);
        if (AccountStatus != AccountStatus.Active) throw new AccountNotActiveException(this.Id,this.AccountStatus);
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new InvalidAmountException(amount);
        if (AccountStatus != AccountStatus.Active) throw new AccountNotActiveException(this.Id, this.AccountStatus);
        if (amount > Balance) throw new InsufficientFundsException(amount,this.Balance);

        ValidateTypeSpecificWithdrawalRules(amount);
        Balance -= amount;
        OnWithdrawalCompleted(amount);
    }
    public void Close()
    {
        if (AccountStatus == AccountStatus.Closed)
            throw new DomainException("This account is already closed");

        if (Balance > 0)
            throw new DomainException("Cannot close account with existing balance");

        AccountStatus = AccountStatus.Closed;
    }

    public void Freeze()
    {
        if (AccountStatus == AccountStatus.Closed)
            throw new DomainException("Cannot freeze a closed account");

        AccountStatus = AccountStatus.Blocked;
    }

    public void Reactivate()
    {
        if (AccountStatus == AccountStatus.Closed)
            throw new DomainException("Cannot reactivate a closed account");

        AccountStatus = AccountStatus.Active;
    }

    public bool CanWithdraw(decimal amount)
    {
        return AccountStatus == AccountStatus.Active && Balance >= amount;
    }

    protected virtual void ValidateTypeSpecificWithdrawalRules(decimal amount) { }
    protected virtual void OnWithdrawalCompleted(decimal amount) { }
    }
}
