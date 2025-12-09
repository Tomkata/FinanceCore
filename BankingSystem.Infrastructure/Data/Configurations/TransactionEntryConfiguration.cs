using BankingSystem.Domain.Aggregates.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure.Data.Configurations
{
    internal class TransactionEntryConfiguration : IEntityTypeConfiguration<TransactionEntry>
    {
        public void Configure(EntityTypeBuilder<TransactionEntry> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
    .HasPrecision(18, 2);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Account)
      .WithMany()
      .HasForeignKey(x => x.AccountId)
      .OnDelete(DeleteBehavior.Restrict);  

            builder.HasOne(x => x.Transaction)
                .WithMany(x => x.TransactionEntries)
                .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Cascade); 

        }
    }
}
