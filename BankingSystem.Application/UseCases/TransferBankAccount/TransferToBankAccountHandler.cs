using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    public class TransferToBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly TransferBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public TransferToBankAccountHandler(
            ICustomerRepository customerRepository,
            TransferBankAccountValidator validator,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(TransferToBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var senderCustomer = await _customerRepository.GetByIdAsync(command.customerId);

            if (senderCustomer is null)
                return Result<Guid>.Failure("Sender customer not found");

            senderCustomer.Transfer(
                command.fromAccountId,
                command.toAccountid,
                command.amount
            );


            await _customerRepository.SaveAsync(senderCustomer);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(command.fromAccountId);
        }
    }
}
