using BankingSystem.Domain.DomainServices;
using System.Numerics;

namespace BankingSystem.Infrastructure.Services
{
    public class FakeIbanGenerator : IIbanGenerator
    {
        public IBAN Generate(Guid id)
        {
            string country = "BG";
            string bankCode = "BANK";
            string branchCode = "0000";
            string accountNumber = id.ToString("N").Substring(0, 10).ToUpper(); // Добави .ToUpper()

            string temp = country + "00" + bankCode + branchCode + accountNumber;
            string checksum = CalculateChecksum(temp);
            string finalIban = country + checksum + bankCode + branchCode + accountNumber;
            return IBAN.Create(finalIban);
        }

        private string CalculateChecksum(string iban)
        {
            string rearranged = iban.Substring(4) + iban.Substring(0, 4);

            var sb = new System.Text.StringBuilder();
            foreach (char c in rearranged)
            {
                if (char.IsLetter(c))
                    sb.Append(IBAN.IbanLetterValues[c]);
                else
                    sb.Append(c);
            }

            int mod97 = 0;
            foreach (char digit in sb.ToString())
                mod97 = (mod97 * 10 + (digit - '0')) % 97;

            int checksum = 98 - mod97;
            return checksum.ToString("00");
        }
    }

}
