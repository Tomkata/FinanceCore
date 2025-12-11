namespace BankingSystem.Infrastructure.Data.Configurations
{
    using BankingSystem.Domain.Aggregates.Customer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValue(new byte[] { 0 });

            builder.ComplexProperty(x => x.IBAN, iban =>
            {
                iban.Property(i => i.Value).HasColumnName("IBAN");
                iban.Property(i => i.CountryCode).HasColumnName("IBANCountryCode");
                iban.Property(i => i.CheckDigits).HasColumnName("IBANCheckDigits");
                iban.Property(i => i.BankCode).HasColumnName("IBANBankCode");
                iban.Property(i => i.AccountNumber).HasColumnName("IBANAccountNumber");
            });

            builder.OwnsOne(x => x.DepositTerm, depositTerm =>
            {
                depositTerm.Property(i => i.Months)
                    .HasColumnName("DepositTermMonths")
                    .IsRequired(false);
            });

            builder.Property(x => x.Balance)
                .HasPrecision(14, 2);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Accounts)
                .HasForeignKey(x => x.CustomerId);
        }
    }
}