using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using BankingSystem.Infrastructure.Services;
using FluentAssertions;

namespace BankingSystem.Tests.Domain
{
    public class AccountBaseTests
    {
        private readonly IIbanGenerator _iban = new FakeIbanGenerator();
        private readonly IAccountFactory _factory = new AccountFactory();

        [Fact]
        public void Deposit_IncreasesBalance()
        {
            var account = _factory.Create(
                AccountType.Checking,
                _iban.Generate(Guid.NewGuid()),
                Guid.NewGuid()
            );

            account.Deposit(100);

            account.Balance.Should().Be(100);
        }

        [Fact]
        public void Deposit_OnClosed_Throws()
        {
            var account = _factory.Create(
                AccountType.Checking,
                _iban.Generate(Guid.NewGuid()),
                Guid.NewGuid()
            );

            account.Close();

            Action act = () => account.Deposit(100);

            act.Should().Throw<AccountNotActiveException>();
        }

        [Fact]
        public void Withdraw_TooMuch_Throws()
        {
            var account = _factory.Create(
                AccountType.Checking,
                _iban.Generate(Guid.NewGuid()),
                Guid.NewGuid()
            );

            account.Deposit(50);

            Action act = () => account.Withdraw(100);

            act.Should().Throw<InsufficientFundsException>();
        }
    }
}
