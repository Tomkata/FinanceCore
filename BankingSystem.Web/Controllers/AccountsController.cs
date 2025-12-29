using BankingSystem.Application.DTOs.Accounts;
using BankingSystem.Application.UseCases.Accounts.CloseBankAccount;
using BankingSystem.Application.UseCases.Accounts.GetAccountById;
using BankingSystem.Application.UseCases.Accounts.GetAllAccountsFromCustomer;
using BankingSystem.Application.UseCases.Accounts.NewFolder;
using BankingSystem.Application.UseCases.Accounts.OpenBankAccount;
using BankingSystem.Application.UseCases.Accounts.ReactiveBankAccount;
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
        public async Task<IActionResult> Close([FromForm] Guid CustomerId,Guid AccountId)
        {


            var command = new FreezeBankAccountCommand(CustomerId,AccountId);

            var result = await _freezeBankAccountHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpPost("reactive")]
        public async Task<IActionResult> Reactive([FromForm] Guid CustomerId, Guid AccountId)
        {


            var command = new ReactiveBankAccountCommand(CustomerId, AccountId);

            var result = await _reactiveBankAccountHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpPost("freeze")]
        public async Task<IActionResult> Freeze([FromForm] Guid CustomerId, Guid AccountId)
        {


            var command = new FreezeBankAccountCommand(CustomerId, AccountId);

            var result = await _freezeBankAccountHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var query = new GetAccountByIdQuery(Id);    
            var result = await _getAccountByIdHandler.Handle(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
            
        }

        [HttpGet("iban")]
        public async Task<IActionResult> GetByIban(string Iban)
        {
            var query = new  GetAccountByIbanQuery(Iban);
            var result = await _getAccountByIbanHandler.Handle(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }

        [HttpGet("customer")]
        public async Task<IActionResult> GetAllByCustomer(Guid Id)
        {
            var query = new GetAllAccountsForCustomerQuery(Id);

            var result = await _getAllAccountsForCustomerHandler.Handle(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }



    }
}