namespace BankingSystem.Application.UseCases.Transactions.SearchTransactions
{
        public record SearchTransactionsQuery(
            Guid? accountId,
            string? transactionType,
            DateTime? startDate,
            DateTime? endDate,
            decimal? minAmount, 
            decimal? maxAmount);
}
