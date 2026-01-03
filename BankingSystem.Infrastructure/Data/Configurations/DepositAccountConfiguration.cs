using BankingSystem.Domain.Aggregates.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DepositAccountConfiguration : IEntityTypeConfiguration<DepositAccount>
{
    public void Configure(EntityTypeBuilder<DepositAccount> builder)
    {
        builder.Property(x => x.MaturityDate)
            .HasColumnName("MaturityDate")
            .IsRequired(false);

        builder.OwnsOne(x => x.DepositTerm, dt =>
        {
            dt.Property(t => t.Months)
                .HasColumnName("DepositTermMonths")
                .IsRequired();

        });
    }
}
