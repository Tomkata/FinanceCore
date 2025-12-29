using BankingSystem.Application.DTOs.Accounts;
using BankingSystem.Application.UseCases.Accounts.GetAccountById;
using BankingSystem.Application.UseCases.Accounts.NewFolder;
using BankingSystem.Application.UseCases.Accounts.OpenBankAccount;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly OpenBankAccountHandler _openAccountHandler;
        private readonly GetAccountByIdHandler _getAccountByIdHandler;
        private readonly GetAccountByIbanHandler  _getAccountByIbanHandler;

        public AccountsController(OpenBankAccountHandler openAccountHandler,
                                   GetAccountByIdHandler getAccountByIdHandler,
                                   GetAccountByIbanHandler getAccountByIbanHandler)
        {
            this._openAccountHandler = openAccountHandler;
            this._getAccountByIdHandler = getAccountByIdHandler;
            this._getAccountByIbanHandler = getAccountByIbanHandler;
        }

        [HttpPost("open")]
        public async Task<IActionResult> Open([FromBody] OpenAccountDto dto)
        {

            var depositTerm = dto.DepositTerm.HasValue ? new DepositTerm(dto.DepositTerm.Value) : null;

            var command = new OpenBankAccountCommand(
                (AccountType)dto.AccountType,
                dto.CustomerId,
                dto.InitialBalance,
                dto.WithdrawLimit,
                depositTerm);

            var result = await _openAccountHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetAccountByIdQuery(id);    
            var result = await _getAccountByIdHandler.Handle(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
            
        }

        [HttpGet("iban")]
        public async Task<IActionResult> GetByIban(string iban)
        {
            var query = new  GetAccountByIbanQuery(iban);
            var result = await _getAccountByIbanHandler.Handle(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }



    }
}