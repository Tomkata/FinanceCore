using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Transaction;
using BankingSystem.Domain.Interfaces;
using Mapster;

namespace BankingSystem.Application.UseCases.Transactions.GetTransactionsByDate
{
    public class GetTransactionsByDateHandler
    {
        private readonly IAccountRepository _accountRepository;

        public GetTransactionsByDateHandler(IAccountRepository accountRepository)
        {
            this._accountRepository = accountRepository;
        }

        public async Task<Result<List<TransactionDto>>> Handle(GetTransactionsByDateQuery query)
        {
            var account = await _accountRepository.GetByIdAsync(query.accountId);
            if (account is null)
                return Result<List<TransactionDto>>.Failure("Account not found.");

            var transactions = account.Transactions
                .Where(x => x.TransactionDate >= query.transactionStartDate &&
                             x.TransactionDate <= query.transactionEndDate)
                .ToList();

            var dto = transactions.Adapt<List<TransactionDto>>();

            return Result<List<TransactionDto>>.Success(dto);
        }
    }
}
