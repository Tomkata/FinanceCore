using BankingSystem.Domain.Aggregates.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystem.Infrastructure.Data.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
   .ValueGeneratedNever();  

            builder.Property(x => x.RowVersion)
              .IsRowVersion()
              .IsConcurrencyToken()
              .ValueGeneratedOnAddOrUpdate();



            builder.HasMany(x => x.TransactionEntries)
        .WithOne(x => x.Transaction)
        .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
