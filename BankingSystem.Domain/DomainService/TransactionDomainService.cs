
using BankingSystem.Domain.Aggregates.Transaction;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Enums.Account;
using BankingSystem.Domain.Enums.Transaction;

public class TransactionDomainService : ITransactionDomainService
{

    private readonly Guid _bankVaultAccountId;

    public TransactionDomainService(Guid bankVaultAccountId)
    {
        _bankVaultAccountId = bankVaultAccountId;
    }

    public Transaction CreateDepositTransaction(Guid accountId, decimal amount)
    {
        var transaction = Transaction.Create(TransactionType.Deposit, $"DEPOSIT {amount}");

        transaction.AddEntry(EntryType.Debit, _bankVaultAccountId, amount, LedgerAccountType.Asset);   
        transaction.AddEntry(EntryType.Credit, accountId, amount, LedgerAccountType.Liability);       


        transaction.Complete();
        return transaction;
    }

    public Transaction CreateWithdrawTransaction(Guid accountId, decimal amount)
    {
        var transaction = Transaction.Create(TransactionType.Withdrawal, $"WITHDRAW {amount}");

        transaction.AddEntry(EntryType.Credit, _bankVaultAccountId, amount, LedgerAccountType.Asset);  
        transaction.AddEntry(EntryType.Debit, accountId, amount, LedgerAccountType.Liability);         

        transaction.Complete();
        return transaction;
    }

    public Transaction CreateTransferTransaction(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        var transaction = Transaction.Create(TransactionType.Transfer, $"TRANSFER {amount}");

        transaction.AddEntry(EntryType.Debit, fromAccountId, amount, LedgerAccountType.Liability);   
        transaction.AddEntry(EntryType.Credit, toAccountId, amount, LedgerAccountType.Liability);     


        transaction.Complete();
        return transaction;
    }
}