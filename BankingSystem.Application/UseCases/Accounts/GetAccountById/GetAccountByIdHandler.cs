

namespace BankingSystem.Application.UseCases.Accounts.GetAccountById
{
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Application.DTOs.Accounts;
    using BankingSystem.Domain.Interfaces;
    using Mapster;
    public class GetAccountByIdHandler
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountByIdHandler(IAccountRepository accountRepository)
        {
            this._accountRepository = accountRepository;
        }

        public async Task<Result<AccountDto>> Handle(GetAccountByIdQuery query)
        {
            var account = await _accountRepository.GetByIdAsync(query.accountId);
            if (account is null)
                return Result<AccountDto>.Failure("Account not found.");

            var dto = account.Adapt<AccountDto>();

            return Result<AccountDto>.Success(dto);

        }
    }
}
