namespace BankingSystem.Application.UseCases.Transactions.GetTransactionsByDate
{
    public record GetTransactionsByDateQuery(
        Guid accountId,
        DateTime transactionStartDate,
        DateTime transactionEndDate,
        int page = 1,
        int pageSize = 20);
    
}
    