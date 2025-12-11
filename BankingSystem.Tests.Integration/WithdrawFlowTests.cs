using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.UseCases.Accounts.WithdrawBankAccount;
using BankingSystem.Domain.Aggregates.Customer;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Customer;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Data;
using BankingSystem.Infrastructure.Persistence;
using BankingSystem.Infrastructure.Repositories;
using BankingSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSystem.Tests.Integration
{
    public class WithdrawFlowTests : IClassFixture<InfrastructureTestFixture>
    {
        private readonly ServiceProvider _services;

        public WithdrawFlowTests(InfrastructureTestFixture fixture)
        {
            _services = fixture.ServiceProvider;
        }

        [Fact]
        public async Task Withdraw_Should_Update_Balance_And_Create_Transaction()
        {
            var db = _services.GetRequiredService<ApplicationDbContext>();
            var uow = _services.GetRequiredService<IUnitOfWork>();
            var customerRepo = _services.GetRequiredService<ICustomerRepository>();
            var ibanGen = _services.GetRequiredService<IIbanGenerator>();

            var customer = new Customer
            (
                "mike12",
                "Mike",
                "Smith",
                new PhoneNumber("+359888555444"),
                new Address("Avenue", "Plovdiv", 4000, "BG"),
                new EGN("1122334455", new DateOnly(1992, 3, 20), Gender.Male)
            );

            var account = customer.OpenAccount(AccountType.Checking,1000,ibanGen);

            await customerRepo.SaveAsync(customer);
            await uow.SaveChangesAsync();

            var handler = new WithdrawBankAccountHandler(
                customerRepo,
                new WithdrawBankAccountValidator(),
                uow
                );

            var command = new WithdrawBankAccountCommand(
                customer.Id,
                200,
                account.Id
                );

            var result = await handler.Handle(command);

            Assert.True(result.IsSuccess);

            var updated = await customerRepo.GetByIdAsync(customer.Id);
            var updatedAccount =  updated.GetAccountById(account.Id);

            Assert.Equal(800,updatedAccount.Balance);
            var transactions = db.Transactions.ToList();
            Assert.Single(transactions);
            Assert.Equal(0, transactions.First().TransactionEntries.Sum(x => x.Amount));


        }
    }
}
