
namespace BankingSystem.Infrastructure.Data.Configurations
{
    using BankingSystem.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.UserName)
    .HasMaxLength(100);

            builder.Property(x => x.FirstName)
    .HasMaxLength(100);

            builder.Property(x => x.LastName)
    .HasMaxLength(100);


            builder.ComplexProperty(x => x.Address, adress =>
            {
                adress.Property(a => a.CityAddress).HasColumnName("CityAddress");
                adress.Property(a => a.City).HasColumnName("City");
                adress.Property(a => a.Zip).HasColumnName("Zip");
                adress.Property(a => a.Country).HasColumnName("Country");
            });

            builder.ComplexProperty(x => x.EGN, egn =>
            {
                egn.Property(e => e.Value).HasColumnName("EGN");
                egn.Property(e => e.BirthDate).HasColumnName("BirthDate");
                egn.Property(e => e.Gender).HasColumnName("Gender");
            });

            builder.ComplexProperty(x => x.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value).HasColumnName("PhoneNumber");
            });


        }
    }
}
