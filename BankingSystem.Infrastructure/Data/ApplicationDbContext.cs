
namespace BankingSystem.Infrastructure.Data
{
    using BankingSystem.Domain.Aggregates.Customer;
    using BankingSystem.Domain.Aggregates.Transaction;
    using BankingSystem.Infrastructure.Data.Configurations;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {}

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {}


        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<TransactionEntry> TransactionEntries { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(
                "Server=.;Database=BankingSystem;TrustServerCertificate=true;Integrated Security=true"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionEntryConfiguration());
        }
    }
}
