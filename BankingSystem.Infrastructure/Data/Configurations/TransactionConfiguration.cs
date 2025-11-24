using BankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystem.Infrastructure.Data.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasIndex(x => x.IdempotencyKey)
            .IsUnique();


            builder.HasMany(x => x.TransactionEntries)
        .WithOne(x => x.Transaction)
        .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
