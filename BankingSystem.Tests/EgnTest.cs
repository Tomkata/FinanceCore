using BankingSystem.Domain.Enums.Account;
using BankingSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Tests.ValueObjects
{
    public class EgnTest
    {
        [Fact]
        public  void ValidEGN_ShoudBeCreatedSuccessfuly()
        {
            string input = "0651035020";
            var egn = EGN.Create(input);
            Assert.Equal(2006, egn.BirthDate.Year);
            Assert.Equal(11, egn.BirthDate.Month);
            Assert.Equal(03, egn.BirthDate.Day);
            Assert.Equal(Gender.Male, egn.Gender);

        }
        [Fact]

        public void InvalidEgn_ShouldThrowException()
        {
            // Arrange
            string wrongEgn = "0051035000";

            // Act + Assert
            Assert.Throws<ArgumentException>(() => EGN.Create(wrongEgn));
        }

        [Theory]
        [InlineData("0651035020")] // valid
        [InlineData("065103502!")] // invalid char
        [InlineData("065103502")]   // too short
        [InlineData("06510350201")] // too long
        public void ValidateVariousEgnFormats(string input)
        {
            if (input == "0651035020")
            {
                var egn = EGN.Create(input);
                Assert.NotNull(egn);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => EGN.Create(input));
            }
        }
    }
}
