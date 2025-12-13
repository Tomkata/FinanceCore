using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BankingSystem.Tests;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+359888123456")]
    [InlineData("+1234567890")]
    [InlineData("0888123456")]
    public void Create_WithValidPhoneNumber_ShouldSucceed(string phoneNumber)
    {
        var result = new PhoneNumber(phoneNumber);

        result.Should().NotBeNull();
        result.Value.Should().Be(phoneNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")] // Too short (less than 10 digits)
    [InlineData("+35988812345abc")] // Contains letters
    [InlineData("088 812 3456")] // Contains spaces
    [InlineData("+3598881234567890123")] // Too long (more than 15 digits)
    public void Create_WithInvalidPhoneNumber_ShouldThrowInvalidPhoneNumberException(string phoneNumber)
    {
        // Act
        var act = () => new PhoneNumber(phoneNumber);

        // Assert
        act.Should().Throw<InvalidPhoneNumberException>();
    }

    [Fact]
    public void PhoneNumbers_WithSameValue_ShouldBeEqual()
    {
        var phone1 = new PhoneNumber("+359888123456");
        var phone2 = new PhoneNumber("+359888123456");

        phone1.Should().Be(phone2);
        (phone1 == phone2).Should().BeTrue();
    }

    [Fact]
    public void PhoneNumbers_WithDifferentValues_ShouldNotBeEqual()
    {
        var phone1 = new PhoneNumber("+359888123456");
        var phone2 = new PhoneNumber("+359888654321");

        phone1.Should().NotBe(phone2);
        (phone1 != phone2).Should().BeTrue();
    }
}
