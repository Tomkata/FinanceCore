using BankingSystem.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BankingSystem.Tests.ValueObjects
{
    public class AddressTests
    {
        [Fact]
        public void Create_WithValidAddress_ShoudSucceed()
        {
            var street = "Main Street 123";
            var city = "Sofia";
            var zip = 1000;
            var country = "Bulgaria";

            var address = new Address(street, city, zip, country);

            address.Should().NotBeNull();
            address.CityAddress.Should().Be(street);
            address.City.Should().Be(city);
            address.Zip.Should().Be(zip);
            address.Country.Should().Be(country);
        }

        [Fact]
        public void Addresses_WithSameValues_ShouldBeEqual()
        {
            var address1 = new Address("Main St", "Sofia", 1000, "BG");
            var address2 = new Address("Main St", "Sofia", 1000, "BG");

            address1.Should().Be(address2);
            (address1 == address2).Should().BeTrue();
        }

        [Fact]
        public void Addresses_WithDifferentCities_ShouldNotBeEqual()
        {
            var address1 = new Address("Main St", "Sofia", 1000, "BG");
            var address2 = new Address("Main St", "Plovdiv", 1000, "BG");

            address1.Should().NotBe(address2);
        }

        [Fact]
        public void Addresses_WithDifferentZipCodes_ShouldNotBeEqual()
        {
            var address1 = new Address("Main St", "Sofia", 1000, "BG");
            var address2 = new Address("Main St", "Sofia", 2000, "BG");

            address1.Should().NotBe(address2);
        }

        [Fact]
        public void Address_IsImmutable()
        {
            var address = new Address("Main St", "Sofia", 1000, "BG");

            address.CityAddress.Should().Be("Main St");
            address.City.Should().Be("Sofia");
            address.Zip.Should().Be(1000);
            address.Country.Should().Be("BG");
        }

    }
}
