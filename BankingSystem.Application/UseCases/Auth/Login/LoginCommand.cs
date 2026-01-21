using BankingSystem.Application.DTOs.Auth;

namespace BankingSystem.Application.UseCases.Auth.Login
{
    public record LoginCommand
    {
        public LoginDto Data { get; set; } = new LoginDto();
    }
}
