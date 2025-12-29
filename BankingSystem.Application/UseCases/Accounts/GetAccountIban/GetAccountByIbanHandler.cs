using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Accounts;
using BankingSystem.Domain.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.UseCases.Accounts.GetAccountByIban
{
    public class GetAccountByIbanHandler
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountByIbanHandler(IAccountRepository accountRepository)
        {
            this._accountRepository = accountRepository;
        }

        public async Task<Result<AccountDto>> Handle(GetAccountByIbanQuery query)
        {

            var account = await _accountRepository.GetByIbanAsync(query.iban);
            if (account is null)
                return Result<AccountDto>.Failure("Account not found.");

            var dto = account.Adapt<AccountDto>();

            return Result<AccountDto>.Success(dto);
        }
    }
}
