
namespace BankingSystem.Application.UseCases.Transactions.GetTransactionsByDate
{
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Application.DTOs.Transaction;
    using BankingSystem.Domain.Interfaces;
    using Mapster;
    using Microsoft.EntityFrameworkCore;

    public class GetTransactionsByDateHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsByDateHandler(IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            this._accountRepository = accountRepository;
            this._transactionRepository = transactionRepository;
        }

        public async Task<Result<PagedResult<TransactionDto>>> Handle(GetTransactionsByDateQuery query)
        {
            var account = await _accountRepository.GetByIdAsync(query.accountId);


            if (account is null)
                return Result<PagedResult<TransactionDto>>.Failure("Account not found.");

            if (query.transactionStartDate > query.transactionEndDate)
                return Result<PagedResult<TransactionDto>>.Failure("Invalid date range.");

            var baseQuery = _transactionRepository
                .Query()
                .Where(x => x.TransactionDate >= query.transactionStartDate &&
                             x.TransactionDate <= query.transactionEndDate &&
                             x.TransactionEntries.Any(x => x.AccountId == query.accountId));

            var totalCount = await baseQuery.CountAsync();

            var items = await baseQuery
                .OrderByDescending(x => x.TransactionDate)
                .Skip((query.page - 1) * query.pageSize)
                .Take(query.pageSize)
                .ProjectToType<TransactionDto>()
                 .ToListAsync();

            var result = new PagedResult<TransactionDto>
            {
                Items = items,
                Page = query.page,
                PageSize = query.pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<TransactionDto>>.Success(result);
        }
    }
}
