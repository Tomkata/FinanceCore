using BankingSystem.Application.UseCases.Transactions.GetTransactionById;
using BankingSystem.Application.UseCases.Transactions.GetTransactionHistoryForAccount;
using BankingSystem.Application.UseCases.Transactions.GetTransactionsByDate;
using BankingSystem.Application.UseCases.Transactions.SearchTransactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly GetTransactionByIdHandler _getTransactionByIdHandler;
        private readonly GetTransactionHistoryForAccountHandler _getTransactionHistoryForAccountHandler;
        private readonly GetTransactionsByDateHandler _getTransactionsByDateHandler;
        private readonly SearchTransactionsHandler _searchTransactionsHandler;

        public TransactionsController(
            GetTransactionByIdHandler getTransactionByIdHandler,
            GetTransactionHistoryForAccountHandler getTransactionHistoryForAccountHandler,
            GetTransactionsByDateHandler getTransactionsByDateHandler,
            SearchTransactionsHandler searchTransactionsHandler)
        {
            _getTransactionByIdHandler = getTransactionByIdHandler;
            _getTransactionHistoryForAccountHandler = getTransactionHistoryForAccountHandler;
            _getTransactionsByDateHandler = getTransactionsByDateHandler;
            _searchTransactionsHandler = searchTransactionsHandler;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetTransactionByIdQuery(id);
            var result = await _getTransactionByIdHandler.Handle(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetHistoryForAccount(Guid accountId)
        {
            var query = new GetTransactionHistoryForAccountQuery(accountId);
            var result = await _getTransactionHistoryForAccountHandler.Handle(query, CancellationToken.None);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }

        [HttpGet("account/{accountId}/date-range")]
        public async Task<IActionResult> GetTransactionsByDateRange(
            Guid accountId,
            DateTime startDate,
            DateTime endDate)
        {
            var query = new GetTransactionsByDateQuery(accountId, startDate, endDate);
            var result = await _getTransactionsByDateHandler.Handle(query, CancellationToken.None);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTransactions(
            [FromQuery] Guid? accountId,
            [FromQuery] string? transactionType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount,
            [FromQuery] string? sortOrder)
        {
            var query = new SearchTransactionsQuery
            {
                AccountId = accountId,
                TransactionType = transactionType,
                StartDate = startDate,
                EndDate = endDate,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                SortOrder = sortOrder
            };

            var result = await _searchTransactionsHandler.Handle(query, CancellationToken.None);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(result.Error);
        }
    }
}
