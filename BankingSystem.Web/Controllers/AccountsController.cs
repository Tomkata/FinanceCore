using BankingSystem.Application.DTOs.Accounts;
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
        private readonly OpenBankAccountHandler handler;

        public AccountsController(OpenBankAccountHandler handler)
        {
            this.handler = handler;
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

            var result = await handler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }
    }
}