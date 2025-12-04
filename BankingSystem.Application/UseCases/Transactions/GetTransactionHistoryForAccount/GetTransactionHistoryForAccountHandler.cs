using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Transaction;
using BankingSystem.Domain.Interfaces;
using Mapster;

namespace BankingSystem.Application.UseCases.Transactions.GetTransactionHistoryForAccount
{
    public class GetTransactionHistoryForAccountHandler
    {
        private readonly IAccountRepository _accountRepository;

        public GetTransactionHistoryForAccountHandler(IAccountRepository accountRepository)
        {
            this._accountRepository = accountRepository;
        }

        public async Task<Result<List<TransactionDto>>> Handle(GetTransactionHistoryForAccountQuery query)
        {
            var account = await _accountRepository.GetByIdAsync(query.accountId);
            if (account is null)
                return Result<List<TransactionDto>>.Failure("Account not found.");

            var transactions = account.Transactions
                .OrderByDescending(x => x.TransactionDate)
                .ToList();

            var dto = transactions.Adapt<List<TransactionDto>>();
                
            return Result<List<TransactionDto>>.Success(dto);


        }
    }
}
