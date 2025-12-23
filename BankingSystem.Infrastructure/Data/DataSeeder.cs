using BankingSystem.Domain.Aggregates;
using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BankingSystem.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, IConfiguration configuration)
        {
            var vaultAccountIdStr = configuration["BankInternalAccounts:VaultAccountId"];
            if (string.IsNullOrEmpty(vaultAccountIdStr) || !Guid.TryParse(vaultAccountIdStr, out var vaultAccountId))
            {
                throw new InvalidOperationException("BankInternalAccounts:VaultAccountId must be configured in appsettings.json");
            }

            var vaultExists = await context.Accounts.AnyAsync(a => a.Id == vaultAccountId);
            if (vaultExists)
            {
                return; 
            }

            var systemCustomerId = Guid.NewGuid();
            var systemCustomerExists = await context.Customers.AnyAsync(c => c.Id == systemCustomerId);

            if (!systemCustomerExists)
            {
                var systemCustomer = Customer.CreateSystemCustomer(
                    systemCustomerId,
                    "SYSTEM",
                    "System",
                    "Account",
                    new PhoneNumber("+359000000000"),
                    new Address("Internal", "Sofia", 1000, "Bulgaria"),
                    EGN.Create("0651035020")
                );

                context.Customers.Add(systemCustomer);
            }

            var vaultIban = IBAN.Create("BG51UNCR70008378815696");

            var vaultAccount = Account.CreateSystemAccount(
                vaultAccountId,
                vaultIban,
                systemCustomerId
            );

            context.Accounts.Add(vaultAccount);
            await context.SaveChangesAsync();

        }
    }
}
