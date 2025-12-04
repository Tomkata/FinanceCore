using BankingSystem.Application.Common.Results;
using BankingSystem.Application.DTOs.Accounts;
using BankingSystem.Domain.Enums.Account;
using BankingSystem.Domain.Interfaces;
using Mapster;


namespace BankingSystem.Application.UseCases.Accounts.GetAllAccountsFromCustomer
{7
    public class GetAllAccountsForCustomerHandler
    {
        private readonly ICustomerRepository _customerRepository;

        public GetAllAccountsForCustomerHandler(ICustomerRepository customerRepository)
        {
            this._customerRepository = customerRepository;
        }

        public async Task<Result<List<AccountDto>>> Handle(GetAllAccountsForCustomerQuery query)
        {
            var customer = await _customerRepository.GetByIdAsync(query.customerId);

            if (customer is null)
                return Result<List<AccountDto>>.Failure("Customer not found.");

            var accounts = customer.Accounts; ;

            var dto = accounts.Adapt<List<AccountDto>>();

            return Result<List<AccountDto>>.Success(dto);
        }
    }
}
