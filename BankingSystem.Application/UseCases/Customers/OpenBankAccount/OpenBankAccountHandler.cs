namespace BankingSystem.Application.UseCases.Customers.OpenBankAccount
{
    using BankingSystem.Application.Common.Interfaces;
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Domain.DomainService;
    using BankingSystem.Domain.DomainServices;
    using BankingSystem.Domain.Interfaces;
    public class OpenBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly OpenBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIbanGenerator _ibanGenerator;
        private readonly IAccountFactory _accountFactory;

        public OpenBankAccountHandler(
                 ICustomerRepository customerRepository,
                 OpenBankAccountValidator validator,
                 IUnitOfWork unitOfWork,
                 IIbanGenerator ibanGenerator,
                 IAccountFactory accountFactory)
        {
            this._customerRepository = customerRepository;
            this._validator = validator;
            this._unitOfWork = unitOfWork;
            this._ibanGenerator = ibanGenerator;
            this._accountFactory = accountFactory;
        }

        public async Task<Result<Guid>> Handle(OpenBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
                return Result<Guid>.Failure(string.Join(", ",validationResult.Errors.Select(x=>x.ErrorMessage)));

            var customer = await _customerRepository.GetByIdAsync(command.customerId);

            if (customer is null)
                return Result<Guid>.Failure("Customer not found");


            var account = customer.OpenAccount(
    command.type,
    command.initialBalance,
    _ibanGenerator,
    _accountFactory,        
    command.withdrawLimit,
    command.term
);


            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(account.Id);
        }
    }
}
