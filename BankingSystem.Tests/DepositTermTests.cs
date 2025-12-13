using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BankingSystem.Tests;

public class DepositTermTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    [InlineData(24)]
    [InlineData(36)]
    public void Create_WithValidMonths_ShouldSucceed(int months)
    {
        var term = new DepositTerm(months);

        term.Should().NotBeNull();
        term.Months.Should().Be(months);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-12)]
    public void Create_WithZeroOrNegativeMonths_ShouldThrowInvalidMonthsException(int months)
    {
        var act = () => new DepositTerm(months);

        act.Should().Throw<InvalidMonthsException>();
    }

    [Fact]
    public void CalculateMaturity_ShouldReturnCorrectMaturityDate()
    {
        var term = new DepositTerm(6); // 6 months
        var startDate = new DateTime(2024, 1, 15);

        var maturityDate = term.CalculateMaturity(startDate);

        maturityDate.Should().Be(new DateTime(2024, 7, 15));
    }

    [Fact]
    public void CalculateMaturity_WithLeapYear_ShouldHandleCorrectly()
    {
        var term = new DepositTerm(12);
        var startDate = new DateTime(2024, 2, 29); 

        var maturityDate = term.CalculateMaturity(startDate);

        maturityDate.Should().Be(new DateTime(2025, 2, 28)); 
    }

    [Fact]
    public void DepositTerms_WithSameMonths_ShouldBeEqual()
    {
        var term1 = new DepositTerm(12);
        var term2 = new DepositTerm(12);

        term1.Should().Be(term2);
    }

    [Fact]
    public void DepositTerms_WithDifferentMonths_ShouldNotBeEqual()
    {
        var term1 = new DepositTerm(12);
        var term2 = new DepositTerm(6);

        term1.Should().NotBe(term2);
    }
}
