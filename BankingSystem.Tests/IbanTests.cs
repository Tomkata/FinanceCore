using BankingSystem.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Tests.ValueObjects
{
    public class IbanTests
    {
        [Fact]
        public void ValidIBAN_ShouldCreateSuccessfully()
        {
            string input = "BG80BNBG96611020345678";

            var iban = IBAN.Create(input);

            Assert.NotNull(iban);
            Assert.Equal("BG",iban.CountryCode);
            Assert.Equal("80",iban.CheckDigits);
            Assert.Equal("BNBG", iban.BankCode);
            Assert.Equal("96611020345678", iban.AccountNumber);
        }

        [Fact]

        public void InvalidChecksum_ShouldThrowException()
        {
            // Arrange
            string wrongIban = "BG00BNBG96611020345678";

            // Act + Assert
            Assert.Throws<IbanException>(() => IBAN.Create(wrongIban));
        }

        [Theory]
        [InlineData("BG80BNBG96611020345678")] // valid
        [InlineData("BG00BNBG96611020345678")] // invalid checksum
        [InlineData("BG80BNBG9!611020345678")] // invalid char
        [InlineData("BG80BNBG966110203456")]   // too short
        [InlineData("BG80BNBG966110203456789")] // too long
        public void ValidateVariousIBANFormats(string input)
        {
            if (input == "BG80BNBG96611020345678")
            {
                var iban = IBAN.Create(input);
                Assert.NotNull(iban);
            }
            else
            {
                Assert.Throws<IbanException>(() => IBAN.Create(input));
            }
        }

    }
}
