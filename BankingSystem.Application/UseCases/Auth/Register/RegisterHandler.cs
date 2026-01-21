using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Auth;
using BankingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Application.UseCases.Auth.Register
{
    public class RegisterHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly RegisterValidator _validator;

        public RegisterHandler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            RegisterValidator validator)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _validator = validator;
        }

        public async Task<Result<RegisterResultDto>> Handle(RegisterCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
                return Result<RegisterResultDto>.Failure(validationResult.ToString());

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(command.Data.Email);
            if (existingUser != null)
                return Result<RegisterResultDto>.Failure("User with this email already exists");

            // Create new user
            var user = new ApplicationUser
            {
                UserName = command.Data.Email,
                Email = command.Data.Email,
                FirstName = command.Data.FirstName,
                LastName = command.Data.LastName,
                CustomerId = command.Data.CustomerId,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, command.Data.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<RegisterResultDto>.Failure($"Failed to create user: {errors}");
            }

            // Get user roles (new users typically have no roles, or you can assign a default role)
            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(user.Id, user.Email!, roles);

            var registerResult = new RegisterResultDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token
            };

            return Result<RegisterResultDto>.Success(registerResult);
        }
    }
}
