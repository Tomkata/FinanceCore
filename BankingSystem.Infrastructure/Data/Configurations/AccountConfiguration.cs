
    namespace BankingSystem.Infrastructure.Data.Configurations
    {
        using BankingSystem.Domain.Aggregates.Customer;
    using BankingSystem.Domain.Enums;
    using Microsoft.EntityFrameworkCore;
        using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System.Reflection.Emit;

        public class AccountConfiguration : IEntityTypeConfiguration<Account>
        {
            public void Configure(EntityTypeBuilder<Account> builder)
            {
                builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
             .ValueGeneratedNever();


        builder
            .HasDiscriminator<AccountType>("Discriminator")
            .HasValue<CheckingAccount>(AccountType.Checking)
            .HasValue<SavingAccount>(AccountType.Saving)
            .HasValue<DepositAccount>(AccountType.Deposit);

            builder.Property(x => x.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate();

                builder.ComplexProperty(x => x.IBAN, iban =>
                {
                    iban.Property(i => i.Value).HasColumnName("IBAN");
                    iban.Property(i => i.CountryCode).HasColumnName("IBANCountryCode");
                    iban.Property(i => i.CheckDigits).HasColumnName("IBANCheckDigits");
                    iban.Property(i => i.BankCode).HasColumnName("IBANBankCode");
                    iban.Property(i => i.AccountNumber).HasColumnName("IBANAccountNumber");
                });


                builder.Property(x => x.Balance)
                    .HasPrecision(14, 2);

                builder.HasOne(x => x.Customer)
                    .WithMany(x => x.Accounts)
                    .HasForeignKey(x => x.CustomerId);

                // Many-to-many: Account <-> Transaction through TransactionEntry
                builder.HasMany(x => x.Transactions)
                    .WithMany()
                    .UsingEntity<BankingSystem.Domain.Aggregates.Transaction.TransactionEntry>(
                        j => j.HasOne(te => te.Transaction)
                              .WithMany(t => t.TransactionEntries)
                              .HasForeignKey(te => te.TransactionId),
                        j => j.HasOne(te => te.Account)
                              .WithMany(a => a.TransactionEntries)
                              .HasForeignKey(te => te.AccountId)
                    );
            }
        }
    }