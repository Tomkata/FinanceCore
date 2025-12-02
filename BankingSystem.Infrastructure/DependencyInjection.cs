

namespace BankingSystem.Infrastructure
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Domain.DomainServics;
    using BankingSystem.Infrastructure.Data;
    using BankingSystem.Infrastructure.Persistence;
    using BankingSystem.Infrastructure.Repositories;
    using BankingSystem.Infrastructure.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
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
            services.Scan(scan => scan
        .FromAssemblyOf<AccountRepository>()
        .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
        .AsImplementedInterfaces()
        .WithScopedLifetime());
            

            //services
            services.AddScoped<IIbanGenerator, IbanGenerator>();

            return services;
        }
    }
}
