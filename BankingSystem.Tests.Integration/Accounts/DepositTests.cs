using Microsoft.Extensions.DependencyInjection;

namespace BankingSystem.Tests.Integration.Accounts
{
    public class DepositTests : IClassFixture<InfrastructureTestFixture>
    {

        private readonly ServiceProvider _services;


        public DepositTests(InfrastructureTestFixture infrastructure)
        {
            _services = infrastructure.ServiceProvider;
        }


        

    }
}
