namespace BankingSystem.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
