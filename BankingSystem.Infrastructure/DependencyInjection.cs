using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Domain.DomainServics;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Infrastructure.Data;
using BankingSystem.Infrastructure.Persistence;
using BankingSystem.Infrastructure.Repositories;
using BankingSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            //db context
            services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //repostories
            services.AddScoped<IAccountRepository,AccountRepository>();
            services.AddScoped<ITransactionRepository,TransactionRepository>();
            services.AddScoped<ICustomerRepository,CustomerRepository>();
            services.AddScoped<IIbanGenerator, IbanGenerator>();

            return services;
        }
    }
}
