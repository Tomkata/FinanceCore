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

            var systemCustomerId = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var systemCustomerExists = await context.Customers.AnyAsync(c => c.Id == systemCustomerId);

            if (!systemCustomerExists)
            {
                var systemCustomer = new Customer(
                    userName: "SYSTEM",
                    firstName: "System",
                    lastName: "Account",
                    phoneNumber: new PhoneNumber("+359000000000"),
                    address: new Address("Internal", "Sofia", 1000, "Bulgaria"),
                    eGN: EGN.Create("0000000000") 
                );

                typeof(Customer).GetProperty("Id")!.SetValue(systemCustomer, systemCustomerId);

                context.Customers.Add(systemCustomer);
            }

            var vaultIban = IBAN.Create("BG00BANK00000000000000");
            var vaultAccount = Account.CreateRegular(vaultIban, systemCustomerId);

            typeof(Account).GetProperty("Id")!.SetValue(vaultAccount, vaultAccountId);

            context.Accounts.Add(vaultAccount);
            await context.SaveChangesAsync();
        }
    }
}
