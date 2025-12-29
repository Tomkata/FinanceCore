using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Accounts;
using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Application.UseCases.Customers.CreateCustomer;
using BankingSystem.Application.UseCases.Customers.DeactivateCustomer;
using BankingSystem.Application.UseCases.Customers.DepositToAccount;
using BankingSystem.Application.UseCases.Customers.GetCustomerByEgn;
using BankingSystem.Application.UseCases.Customers.GetCustomerById;
using BankingSystem.Application.UseCases.Customers.OpenBankAccount;
using BankingSystem.Application.UseCases.Customers.WithdrawFromAccount;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;

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

        public CustomersController(
            CreateCustomerHandler createCustomerHandler, 
            DeactivateCustomerCommandHandler deactivateCustomerCommandHandler,
            DepositToBankAccountHandler depositToBankAccountHandler, 
            WithdrawToBankAccountHandler withdrawToBankAccountHandler, 
            GetCustomerByEgnHandler getCustomerByEgnHandler, 
            GetCustomerByIdHandler getCustomerByIdHandler,
            OpenBankAccountHandler openBankAccountHandler)
        {
            this._createCustomerHandler = createCustomerHandler;
            this._deactivateCustomerCommandHandler = deactivateCustomerCommandHandler;
            this._depositToBankAccountHandler = depositToBankAccountHandler;
            this._withdrawToBankAccountHandler = withdrawToBankAccountHandler;
            this._getCustomerByEgnHandler = getCustomerByEgnHandler;
            this._getCustomerByIdHandler = getCustomerByIdHandler;
            this._openBankAccountHandler = openBankAccountHandler;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto Dto)
        {
            var command = new CreateCustomerCommand
            {
                Data = Dto
            };

            var result = await _createCustomerHandler.Handle(command);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Error);
        }

        [HttpPost("open")]
        public async Task<IActionResult> Open([FromBody] OpenAccountDto Dto)
        {

            var depositTerm = Dto.DepositTerm.HasValue ? new DepositTerm(Dto.DepositTerm.Value) : null;

            var command = new OpenBankAccountCommand(
                (AccountType)Dto.AccountType,
                Dto.CustomerId,
                Dto.InitialBalance,
                Dto.WithdrawLimit,
                depositTerm);

            var result = await _openBankAccountHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpPost("deactive")]
        public async Task<IActionResult> Deactive([FromForm] Guid Id)
        {
            var command = new DeactivateCustomerCommand(Id);

            var result = await _deactivateCustomerCommandHandler.Handle(command);
            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }


    }
}
