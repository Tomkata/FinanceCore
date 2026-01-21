using BankingSystem.Application.DTOs.Auth;
using BankingSystem.Application.UseCases.Auth.Login;
using BankingSystem.Application.UseCases.Auth.Register;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RegisterHandler _registerHandler;
        private readonly LoginHandler _loginHandler;

        public AuthController(
            RegisterHandler registerHandler,
            LoginHandler loginHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var command = new RegisterCommand { Data = dto };
            var result = await _registerHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var command = new LoginCommand { Data = dto };
            var result = await _loginHandler.Handle(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Error);
        }
    }
}
