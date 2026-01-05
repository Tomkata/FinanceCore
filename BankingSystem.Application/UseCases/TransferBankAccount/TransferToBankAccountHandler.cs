using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    public class TransferToBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly TransferBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransferDomainService _transferDomainService;

        public TransferToBankAccountHandler(
            ICustomerRepository customerRepository,
            TransferBankAccountValidator validator,
            IUnitOfWork unitOfWork,
            ITransferDomainService transferDomainService)
        {
            _customerRepository = customerRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _transferDomainService = transferDomainService;
        }

        public async Task<Result<Guid>> Handle(TransferBankAccountCommand command)
        {
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Result<Guid>.Failure(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var senderCustomer = await _customerRepository.GetByIdAsync(command.senderCustomerId);
            var reciver = await _customerRepository.GetByIdAsync(command.receiverCustomerId);

            if (senderCustomer is null)
                return Result<Guid>.Failure("Sender customer not found");
            if (reciver is null)
                return Result<Guid>.Failure("Sender customer not found");

            _transferDomainService.Transfer(
                senderCustomer,
                command.fromAccountId,
                reciver,
                command.toAccountId,
                command.amount
                );


            await _customerRepository.SaveAsync(senderCustomer);
            await _customerRepository.SaveAsync(reciver);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(command.fromAccountId);
        }
    }
}
