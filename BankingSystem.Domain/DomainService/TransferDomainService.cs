using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Aggregates.Customer.Events;
using BankingSystem.Domain.Exceptions;

namespace BankingSystem.Domain.DomainService
{
    public class TransferDomainService : ITransferDomainService
    {

        public void Transfer(Customer sender, Guid senderAccountId, 
            Customer recipient, Guid recipentAccountId,
            decimal amount)
        {
            if (sender is null) throw new ArgumentNullException();
            if (recipient is null) throw new ArgumentNullException();

            if (senderAccountId == recipentAccountId)
                throw new CannotTransferToSameAccountException();

            sender.Withdraw(senderAccountId,amount);
            recipient.Deposit(recipentAccountId, amount);

           
        }
    }
}
