using BankingSystem.Domain.Aggregates.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SavingAccountConfiguration : IEntityTypeConfiguration<SavingAccount>
{
    public void Configure(EntityTypeBuilder<SavingAccount> builder)
    {
        builder.Property(x => x.WithdrawLimits)
            .HasColumnName("WithdrawLimits")
            .IsRequired();

        builder.Property(x => x.CurrentMonthWithdrawals)
            .HasColumnName("CurrentMonthWithdrawals")
            .HasDefaultValue(0);

        builder.Property(x => x.LastWithdrawalDate)
            .HasColumnName("LastWithdrawalDate")
            .IsRequired(false);
    }
}
