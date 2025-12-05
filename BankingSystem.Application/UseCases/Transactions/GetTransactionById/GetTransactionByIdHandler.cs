

namespace BankingSystem.Application.UseCases.Transactions.GetTransactionById
{
    using BankingSystem.Application.Common.Results;
    using BankingSystem.Application.DTOs.Transaction;
    using BankingSystem.Domain.Interfaces;
    using Mapster;
    public class GetTransactionByIdHandler
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionByIdHandler(ITransactionRepository transactionRepository)
        {
            this._transactionRepository = transactionRepository;
        }


        public async Task<Result<TransactionDto>> Handle(GetTransactionByIdQuery query)
        {
            var transaction = await _transactionRepository.GetByIdAsync(query.transactionId);
            if (transaction is null)
                return Result<TransactionDto>.Failure("Transaction not found.");

            var dto = transaction.Adapt<TransactionDto>();

            return Result<TransactionDto>.Success(dto);
        }
    }
}
