

namespace BankingSystem.Infrastructure
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Application.DomainEventHandlers;
    using BankingSystem.Domain.Common;
    using BankingSystem.Domain.DomainService;
    using BankingSystem.Domain.DomainServices;
    using BankingSystem.Infrastructure.Data;
    using BankingSystem.Infrastructure.DomainEvents;
    using BankingSystem.Infrastructure.Identity;
    using BankingSystem.Infrastructure.Persistence;
    using BankingSystem.Infrastructure.Repositories;
    using BankingSystem.Infrastructure.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            //db context
            services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //identity
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            //jwt authentication
            var jwtSecret = configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT Secret not configured in appsettings.json");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

            //jwt token service
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            //unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //repostories
            services.Scan(scan => scan
        .FromAssemblyOf<AccountRepository>()
        .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
        .AsImplementedInterfaces()
        .WithScopedLifetime());


            //domain services
            services.AddScoped<IAccountFactory, AccountFactory>();
            services.AddScoped<ITransferDomainService, TransferDomainService>();

            //services - Use FakeIbanGenerator for now (has proper checksum validation)
            // TODO: Implement real IbanGenerator with bank integration, then use environment-based registration
            services.AddScoped<IIbanGenerator, FakeIbanGenerator>();

            //event dispatcher
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            //domain event handlers
            services.Scan(scan => scan
            .FromAssembliesOf(typeof(AccountCreditedEventHandler))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            //domain service - configured with vault account ID from appsettings
            services.AddScoped<ITransactionDomainService, TransactionDomainService>(sp =>
            {
                var vaultAccountId = configuration["BankInternalAccounts:VaultAccountId"];
                if (string.IsNullOrEmpty(vaultAccountId) || !Guid.TryParse(vaultAccountId, out var vaultId))
                {
                    throw new InvalidOperationException("BankInternalAccounts:VaultAccountId must be configured in appsettings.json");
                }
                return new TransactionDomainService(vaultId);
            });

            return services;
        }
    }
}
