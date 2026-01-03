using BankingSystem.Domain.Aggregates.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
   .ValueGeneratedNever();

        // RowVersion - конфигуриран САМО веднъж
        builder.Property(x => x.RowVersion)
    .IsRowVersion()                    // Concurrency token
    .IsConcurrencyToken()              // Mark for optimistic locking
    .ValueGeneratedOnAddOrUpdate()     // EF manages the value
    .IsRequired(false);

        builder.Property(x => x.UserName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.ComplexProperty(x => x.Address, adress =>
        {
            adress.Property(a => a.CityAddress).HasColumnName("CityAddress");
            adress.Property(a => a.City).HasColumnName("City");
            adress.Property(a => a.Zip).HasColumnName("Zip");
            adress.Property(a => a.Country).HasColumnName("Country");
        });

        builder.ComplexProperty(x => x.EGN, egn =>
        {
            egn.Property(e => e.Value)
                .HasColumnName("EGN")
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsRequired();
            egn.Property(e => e.BirthDate)
                .HasColumnName("BirthDate")
                .IsRequired();
            egn.Property(e => e.Gender)
                .HasColumnName("Gender")
                .IsRequired();
        });

        builder.HasIndex(x => x.UserName)
            .IsUnique();

        builder.ComplexProperty(x => x.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20);
        });
    }
}