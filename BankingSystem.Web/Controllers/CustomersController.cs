using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Accounts;
using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Application.DTOs.Transfer;
using BankingSystem.Application.UseCases.Customers.CreateCustomer;
using BankingSystem.Application.UseCases.Customers.DeactivateCustomer;
using BankingSystem.Application.UseCases.Customers.DepositToAccount;
using BankingSystem.Application.UseCases.Customers.GetCustomerByEgn;
using BankingSystem.Application.UseCases.Customers.GetCustomerById;
using BankingSystem.Application.UseCases.Customers.OpenBankAccount;
using BankingSystem.Application.UseCases.Customers.WithdrawFromAccount;
using BankingSystem.Application.UseCases.TransferBankAccount;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.AccessControl;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BankingSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CreateCustomerHandler  _createCustomerHandler;
        private readonly DeactivateCustomerCommandHandler _deactivateCustomerCommandHandler;
        private readonly DepositToBankAccountHandler _depositToBankAccountHandler;
        private readonly WithdrawToBankAccountHandler _withdrawToBankAccountHandler;
        private readonly GetCustomerByEgnHandler _getCustomerByEgnHandler;
        private readonly GetCustomerByIdHandler _getCustomerByIdHandler;
        private readonly OpenBankAccountHandler _openBankAccountHandler;
        private readonly TransferToBankAccountHandler _transferToBankAccountHandler;

        public CustomersController(
            CreateCustomerHandler createCustomerHandler, 
            DeactivateCustomerCommandHandler deactivateCustomerCommandHandler,
            DepositToBankAccountHandler depositToBankAccountHandler, 
            WithdrawToBankAccountHandler withdrawToBankAccountHandler, 
            GetCustomerByEgnHandler getCustomerByEgnHandler, 
            GetCustomerByIdHandler getCustomerByIdHandler,
            OpenBankAccountHandler openBankAccountHandler,
            TransferToBankAccountHandler transferToBankAccountHandler)
        {
            this._createCustomerHandler = createCustomerHandler;
            this._deactivateCustomerCommandHandler = deactivateCustomerCommandHandler;
            this._depositToBankAccountHandler = depositToBankAccountHandler;
            this._withdrawToBankAccountHandler = withdrawToBankAccountHandler;
            this._getCustomerByEgnHandler = getCustomerByEgnHandler;
            this._getCustomerByIdHandler = getCustomerByIdHandler;
            this._openBankAccountHandler = openBankAccountHandler;
            this._transferToBankAccountHandler = transferToBankAccountHandler;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            var command = new CreateCustomerCommand
            {
                Data = dto
            };

            var result = await _createCustomerHandler.Handle(command);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Error);
        }

        [HttpPost("openAccount")]
        public async Task<IActionResult> Open([FromBody] OpenAccountDto dto)
        {

            var depositTerm = dto.DepositTerm.HasValue ? new DepositTerm(dto.DepositTerm.Value) : null;

            var command = new OpenBankAccountCommand(
                (AccountType)dto.AccountType,
                dto.CustomerId,
                dto.InitialBalance,
                dto.WithdrawLimit,
                depositTerm);

            var result = await _openBankAccountHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpPost("deactiveCustomer")]
        public async Task<IActionResult> Deactive([FromForm] Guid id)
        {
            var command = new DeactivateCustomerCommand(id);

            var result = await _deactivateCustomerCommandHandler.Handle(command);
            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromForm] Guid customerId, Guid accountId,decimal amount)
        {
            var command = new DepositBankAccountCommand(customerId,accountId,amount);

            var result = await _depositToBankAccountHandler.Handle(command);
            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromForm] Guid customerId, Guid accoundId, decimal amount)
        {
            var command = new WithdrawBankAccountCommand(customerId,amount,accoundId);

            var result = await _withdrawToBankAccountHandler.Handle(command);
            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpGet("searchByEgn")]
        public async Task<IActionResult> SearchByEgn( string egn)
        {
            var query = new GetCustomerByEgnQuery(egn);

            var result = await _getCustomerByEgnHandler.Handle(query);
            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
                    
        }

        [HttpGet("searchById")]
        public async Task<IActionResult> SearchById(Guid id)
        {
            var query = new GetCustomerByIdQuery(id);

            var result = await _getCustomerByIdHandler.Handle(query);
            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);

        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferDto dto)
        {
            var command = new TransferBankAccountCommand(
                 dto.SenderCustomerId,
                 dto.ReceiverCustomerId,
                 dto.FromAccountId,
                 dto.ToAccountId,
                 dto.Amount
 );

            var result = await _transferToBankAccountHandler.Handle(command);
            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }


    }
}
