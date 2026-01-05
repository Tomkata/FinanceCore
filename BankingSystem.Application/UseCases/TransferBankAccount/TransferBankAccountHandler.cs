using BankingSystem.Application.Common.Interfaces;
using BankingSystem.Application.Common.Results;
using BankingSystem.Domain.DomainService;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Application.UseCases.TransferBankAccount
{
    public class TransferBankAccountHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly TransferBankAccountValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransferDomainService _transferDomainService;

        public TransferBankAccountHandler(
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

            var sender = await _customerRepository.GetByIdAsync(command.SenderCustomerId);
            var receiver = await _customerRepository.GetByIdAsync(command.ReceiverCustomerId);

            if (sender is null)
                return Result<Guid>.Failure("Sender customer not found");

            if (receiver is null)
                return Result<Guid>.Failure("Receiver customer not found");

            // Use domain service to coordinate cross-aggregate operation
            _transferDomainService.Transfer(
                sender,
                command.FromAccountId,
                receiver,
                command.ToAccountId,
                command.Amount
            );

            await _customerRepository.SaveAsync(sender);
            await _customerRepository.SaveAsync(receiver);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Success(command.FromAccountId);
        }
    }
}
