using BankingSystem.Domain.Aggregates.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystem.Infrastructure.Data.Configurations
{
    internal class TransactionEntryConfiguration : IEntityTypeConfiguration<TransactionEntry>
    {
        public void Configure(EntityTypeBuilder<TransactionEntry> builder)
        {
            builder.HasKey(x => x.Id);  

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Property(x => x.RowVersion)
    .IsRowVersion()                    // Concurrency token
    .IsConcurrencyToken()              // Mark for optimistic locking
    .ValueGeneratedOnAddOrUpdate()     // EF manages the value
    .IsRequired(false);

            builder.HasOne(x => x.Account)
                .WithMany() // Removed lambda to fix CS0266 and CS1662
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Transaction)
                .WithMany(x => x.TransactionEntries)
                .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}