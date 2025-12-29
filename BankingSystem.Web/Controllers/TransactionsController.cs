using BankingSystem.Application.UseCases.Transactions.GetTransactionById;
using BankingSystem.Application.UseCases.Transactions.GetTransactionHistoryForAccount;
using BankingSystem.Application.UseCases.Transactions.GetTransactionsByDate;
using BankingSystem.Application.UseCases.Transactions.SearchTransactions;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Web.Controllers
{
    public class TransactionsController : ControllerBase
    {
        private readonly GetTransactionByIdHandler _getTransactionByIdHandler;
        private readonly GetTransactionHistoryForAccountHandler _getTransactionHistoryForAccountHandler;
        private readonly GetTransactionsByDateHandler _getTransactionsByDateHandler;
        private readonly SearchTransactionsHandler _searchTransactionsHandler;
    }
}
