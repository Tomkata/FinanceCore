

namespace BankingSystem.Application.UseCases.Transactions.SearchTransactions
{
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Application.DTOs.Transaction;
    using BankingSystem.Domain.Entities;
    using BankingSystem.Domain.Interfaces;
    using Mapster;
    using Microsoft.EntityFrameworkCore;

    public class SearchTransactionsHandler
    {
        private readonly ITransactionRepository _transactionRepository;

        public SearchTransactionsHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Result<List<TransactionDto>>> Handle(
            SearchTransactionsQuery query,
            CancellationToken cancellationToken)
        {
            if (query.StartDate > query.EndDate)
                return Result<List<TransactionDto>>.Failure("Invalid date range.");

            if (query.MinAmount > query.MaxAmount)
                return Result<List<TransactionDto>>.Failure("Invalid amount range.");

            var queryable = _transactionRepository.Query();

            queryable = ApplyFilters(queryable, query);

            var transactions = await queryable
                .ProjectToType<TransactionDto>()
                .ToListAsync(cancellationToken);

            return Result<List<TransactionDto>>.Success(transactions);
        }

        private IQueryable<Transaction> ApplyFilters(
            IQueryable<Transaction> queryable,
            SearchTransactionsQuery query)
        {

            if (query.AccountId.HasValue)
            {
                queryable = queryable.Where(x =>
                    x.TransactionEntries.Any(te => te.AccountId == query.AccountId));
            }

            if (!string.IsNullOrEmpty(query.TransactionType))
            {
                queryable = queryable.Where(x =>
                    x.TransactionType.ToString() == query.TransactionType);
            }

            if (query.StartDate.HasValue)
            {
                queryable = queryable.Where(x => x.CreatedAt >= query.StartDate);
            }

            if (query.EndDate.HasValue)
            {
                queryable = queryable.Where(x => x.CreatedAt <= query.EndDate);
            }

            if (query.MinAmount.HasValue)
            {
                queryable = queryable.Where(x =>
                    x.TransactionEntries.Any(te => te.Amount >= query.MinAmount));
            }

            if (query.MaxAmount.HasValue)
            {
                queryable = queryable.Where(x =>
                    x.TransactionEntries.Any(te => te.Amount <= query.MaxAmount));
            }

            return queryable;
        }
    }
}
