

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

        public async Task<Result<PagedResult<TransactionDto>>> Handle(
            SearchTransactionsQuery query,
            CancellationToken cancellationToken)
        {
            if (query.StartDate > query.EndDate)
                return Result<PagedResult<TransactionDto>>.Failure("Invalid date range.");

            if (query.MinAmount > query.MaxAmount)
                return Result<PagedResult<TransactionDto>>.Failure("Invalid amount range.");

            var baseQuery = _transactionRepository.Query();
            baseQuery = ApplyFilters(baseQuery, query);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectToType<TransactionDto>()
                .ToListAsync(cancellationToken);

            var result = new PagedResult<TransactionDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<TransactionDto>>.Success(result);
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
