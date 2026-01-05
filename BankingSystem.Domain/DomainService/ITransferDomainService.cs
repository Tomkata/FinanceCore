using BankingSystem.Domain.Aggregates.Customer;
using System.Net.Mail;

namespace BankingSystem.Domain.DomainService
{
    public interface ITransferDomainService
    {
        //I can access the accounts from customers
        public void Transfer(Customer sender,Guid senderAccountId, 
            Customer recipient, Guid recipentAccountId,
            decimal amount);
    }
}
