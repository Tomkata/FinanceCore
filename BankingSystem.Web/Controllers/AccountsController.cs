using BankingSystem.Application.DTOs.Accounts;
using BankingSystem.Application.UseCases.Accounts.CloseBankAccount;
using BankingSystem.Application.UseCases.Accounts.GetAccountById;
using BankingSystem.Application.UseCases.Accounts.GetAllAccountsFromCustomer;
using BankingSystem.Application.UseCases.Accounts.GetAccountByIban;
using BankingSystem.Application.UseCases.Accounts.ReactiveBankAccount;
using BankingSystem.Application.UseCases.Customers.OpenBankAccount;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly OpenBankAccountHandler _openAccountHandler;
        private readonly FreezeBankAccountHandler _freezeBankAccountHandler;
        private readonly ReactiveBankAccountHandler   _reactiveBankAccountHandler;
        private readonly GetAccountByIdHandler _getAccountByIdHandler;
        private readonly GetAccountByIbanHandler _getAccountByIbanHandler;
        private readonly GetAllAccountsForCustomerHandler _getAllAccountsForCustomerHandler;

        public AccountsController(OpenBankAccountHandler openAccountHandler,
                                   GetAccountByIdHandler getAccountByIdHandler,
                                   GetAccountByIbanHandler getAccountByIbanHandler,
                                   FreezeBankAccountHandler freezeBankAccountHandler,
                                   ReactiveBankAccountHandler reactiveBankAccountHandler,
                                   GetAllAccountsForCustomerHandler getAllAccountsForCustomerHandler)
        {
            this._openAccountHandler = openAccountHandler;
            this._getAccountByIdHandler = getAccountByIdHandler;
            this._getAccountByIbanHandler = getAccountByIbanHandler;
            this._freezeBankAccountHandler = freezeBankAccountHandler;
            this._reactiveBankAccountHandler = reactiveBankAccountHandler;
            this._getAllAccountsForCustomerHandler = getAllAccountsForCustomerHandler;
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

        [HttpPost("close")]
        public async Task<IActionResult> Close([FromBody] CloseAccountDto dto)
        {
            var command = new FreezeBankAccountCommand(dto.CustomerId, dto.AccountId);

            var result = await _freezeBankAccountHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpPost("reactivate")]
        public async Task<IActionResult> Reactivate([FromBody] ReactivateAccountDto dto)
        {
            var command = new ReactiveBankAccountCommand(dto.CustomerId, dto.AccountId);

            var result = await _reactiveBankAccountHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpPost("freeze")]
        public async Task<IActionResult> Freeze([FromBody] FreezeAccountDto dto)
        {
            var command = new FreezeBankAccountCommand(dto.CustomerId, dto.AccountId);

            var result = await _freezeBankAccountHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetAccountByIdQuery(id);
            var result = await _getAccountByIdHandler.Handle(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);

        }

        [HttpGet("iban/{iban}")]
        public async Task<IActionResult> GetByIban(string iban)
        {
            var query = new  GetAccountByIbanQuery(iban);
            var result = await _getAccountByIbanHandler.Handle(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetAllByCustomer(Guid customerId)
        {
            var query = new GetAllAccountsForCustomerQuery(customerId);

            var result = await _getAllAccountsForCustomerHandler.Handle(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }



    }
}