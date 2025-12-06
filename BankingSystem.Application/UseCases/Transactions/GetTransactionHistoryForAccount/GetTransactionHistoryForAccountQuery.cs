using BankingSystem.Application.Common.Results;

namespace BankingSystem.Application.UseCases.Transactions.GetTransactionHistoryForAccount
{
    public record GetTransactionHistoryForAccountQuery(Guid accountId, int page = 1, int pageSize = 20);

}
