using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BankingSystem.Tests;

public class IbanTests
{
    [Fact]
    public void Create_WithValidIBAN_ShouldSuccessfully()
    {
        string input = "BG80BNBG96611020345678";

        var iban = IBAN.Create(input);

        // Assert
        iban.Should().NotBeNull();
        iban.Value.Should().Be(input);
        iban.CountryCode.Should().Be("BG");
        iban.CheckDigits.Should().Be("80");
        iban.BankCode.Should().Be("BNBG");
        iban.AccountNumber.Should().Be("96611020345678");
    }

    [Fact]
    public void Create_WithInvalidChecksum_ShouldThrowIbanException()
    {
        string wrongIban = "BG00BNBG96611020345678"; // Invalid checksum

        var act = () => IBAN.Create(wrongIban);

        act.Should().Throw<IbanException>()
            .WithMessage("*checksum*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrEmptyIBAN_ShouldThrowIbanException(string input)
    {
        var act = () => IBAN.Create(input);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("BG80BNBG9!611020345678")]  // Invalid character
    [InlineData("BG80BNBG966110203456")]    // Too short
    [InlineData("BG80BNBG966110203456789")] // Too long
    [InlineData("XX80BNBG96611020345678")]  // Invalid country code (not BG)
    public void Create_WithInvalidFormat_ShouldThrowIbanException(string input)
    {
        // Act
        var act = () => IBAN.Create(input);

        // Assert
        act.Should().Throw<IbanException>();
    }

    [Fact]
    public void IBANs_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var iban1 = IBAN.Create("BG80BNBG96611020345678");
        var iban2 = IBAN.Create("BG80BNBG96611020345678");

        // Assert
        iban1.Should().Be(iban2);
        (iban1 == iban2).Should().BeTrue();
    }

    [Fact]
    public void IBANs_WithDifferentValues_ShouldNotBeEqual()
    {
        var iban1 = IBAN.Create("BG80BNBG96611020345678");
        var iban2 = IBAN.Create("BG18FINV91501014464646"); 

        iban1.Should().NotBe(iban2);
        (iban1 != iban2).Should().BeTrue();
    }

    [Fact]
    public void IBAN_IsImmutable()
    {
        var iban = IBAN.Create("BG80BNBG96611020345678");

        iban.Value.Should().Be("BG80BNBG96611020345678");
        iban.CountryCode.Should().Be("BG");
    }

    [Theory]
    [InlineData("BG05BNPA94401473621817")]
    [InlineData("BG74IORT80948394244858 ")]
    [InlineData("BG59UNCR70004762841113")]
    public void Create_WithMultipleValidBulgarianIBANs_ShouldSucceed(string validIban)
    {
        var iban = IBAN.Create(validIban);

        iban.Should().NotBeNull();
        iban.CountryCode.Should().Be("BG");
        iban.Value.Length.Should().Be(22); 
    }
}
