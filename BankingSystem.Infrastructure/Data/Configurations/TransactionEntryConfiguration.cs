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
                .IsRowVersion()
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            // Relationships are configured in AccountConfiguration.UsingEntity
        }
    }
}