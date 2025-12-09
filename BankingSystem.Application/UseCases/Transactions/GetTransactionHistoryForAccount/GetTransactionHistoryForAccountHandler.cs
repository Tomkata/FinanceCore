using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Transaction;
using BankingSystem.Domain.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace BankingSystem.Application.UseCases.Transactions.GetTransactionHistoryForAccount
{
    public class GetTransactionHistoryForAccountHandler
    {
        private readonly ITransactionRepository  _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public GetTransactionHistoryForAccountHandler(ITransactionRepository transactionRepository,
                                                       IAccountRepository accountRepository)
        {
            this._transactionRepository = transactionRepository;
            this._accountRepository = accountRepository;
        }

        public async Task<Result<PagedResult<TransactionDto>>> Handle(GetTransactionHistoryForAccountQuery query, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(query.accountId);
            if (account is null)
                return Result<PagedResult<TransactionDto>>.Failure("Account not found.");


            var baseQuery = _transactionRepository
                .Query()
                .Where(x=>x.TransactionEntries.Any(x=>x.AccountId == query.accountId));




            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .OrderByDescending(x => x.TransactionDate)
                .Skip((query.page - 1) * query.pageSize)
                .Take(query.pageSize)
                .ProjectToType<TransactionDto>()
                .ToListAsync(cancellationToken);


            

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
