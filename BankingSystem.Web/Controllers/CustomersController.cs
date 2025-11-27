using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Customer;
using BankingSystem.Application.UseCases.Customers.CreateCustomer;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CreateCustomerHandler _handler;

        public CustomersController(CreateCustomerHandler handler)
        {
            this._handler = handler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            var command = new CreateCustomerCommand
            {
                Data = dto
            };

            var result = await _handler.Handle(command);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Error);
        }
    }
}
