using BankingSystem.Application.DTOs.Auth;

namespace BankingSystem.Application.UseCases.Auth.Register
{
    public record RegisterCommand
    {
        public RegisterDto Data { get; set; } = new RegisterDto();
    }
}
