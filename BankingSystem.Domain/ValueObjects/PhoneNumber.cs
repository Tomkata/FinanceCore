

namespace BankingSystem.Domain.ValueObjects
{
    using BankingSystem.Domain.Exceptions;
    using System.Text.RegularExpressions;
    public record class PhoneNumber
    {
        private PhoneNumber()
        {  }
        public PhoneNumber(string value)
        {
            if (!IsValid(value)) throw new InvalidPhoneNumberException(value);

            this.Value = value;
        }
            
        private bool IsValid(string value)
        {
            return !String.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^\+?[0-9]{10,15}$");
        }

        public string Value { get; init; }

    }
}
