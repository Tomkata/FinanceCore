namespace BankingSystem.Application.UseCases.Transactions.GetTransactionsByDate
{
    public record GetTransactionsByDateQuery(Guid accountId,
        DateTime transactionStartDate, 
        DateTime transactionEndDate);
    
}
    