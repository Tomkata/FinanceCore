using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Auth;
using BankingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Application.UseCases.Auth.Login
{
    public class LoginHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly LoginValidator _validator;

        public LoginHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService,
            LoginValidator validator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _validator = validator;
        }

        public async Task<Result<LoginResultDto>> Handle(LoginCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
                return Result<LoginResultDto>.Failure(validationResult.ToString());

            // Find user by email
            var user = await _userManager.FindByEmailAsync(command.Data.Email);
            if (user == null)
                return Result<LoginResultDto>.Failure("Invalid email or password");

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, command.Data.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Result<LoginResultDto>.Failure("Invalid email or password");

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(user.Id, user.Email!, roles);

            var loginResult = new LoginResultDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token,
                CustomerId = user.CustomerId
            };

            return Result<LoginResultDto>.Success(loginResult);
        }
    }
}
