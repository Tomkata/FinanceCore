
namespace BankingSystem.Application.UseCases.Accounts.GetAccountWithRecentTransactions
{
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Application.DTOs.Accounts;
    using BankingSystem.Application.DTOs.Transaction;
    using BankingSystem.Domain.Entities;
    using BankingSystem.Domain.Interfaces;
    using Mapster;
    using System.Collections.Generic;

    public class GetAccountWithRecentTransactionsHandler
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountWithRecentTransactionsHandler(IAccountRepository accountRepository)
        {
            this._accountRepository = accountRepository;
        }

        public async Task<Result<AccountWithRecentTransactionsDto>> Handle(GetAccountWithRecentTransactionsQuery query)
        {
            var account = await _accountRepository.GetByIdAsync(query.accountId);
            if (account is null)
                return Result<AccountWithRecentTransactionsDto>.Failure("Account not found.");

            var lastFive = account.Transactions
    
                .OrderByDescending(x => x.TransactionDate)
                .Take(5)
                .ToList();

            var dto = new AccountWithRecentTransactionsDto
            {
                Id = account.Id,
                Balance = account.Balance,
                AccountType = account.AccountType.ToString(),
                AccountStatus = account.AccountStatus.ToString(),
                IBAN = account.IBAN.Value,
                RecentTransactions = lastFive.Adapt<List<TransactionDto>>()
            };


            return Result<AccountWithRecentTransactionsDto>.Success(dto);
        }

    }
}
