
namespace BankingSystem.Domain.ValueObjects
{
    using BankingSystem.Domain.Enums;
    using System.Runtime.CompilerServices;

    public  record class EGN
    {
        private EGN()
        {  }
        public string Value { get; init; }

        public DateOnly BirthDate { get; init; }
        public Gender Gender { get; init; }

        public EGN(string value, DateOnly birthDate, Gender gender)
        {
           this.Value = value;
           this.BirthDate = birthDate;
           this.Gender = gender;
        }

        public static EGN Create(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                throw new ArgumentException("EGN cannot be empty.");
            if (raw.Length != 10 || !raw.All(char.IsDigit))
                throw new ArgumentException("EGN must be exactly 10 digits.");

            int[] weights = { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (raw[i] - '0') * weights[i];
            int control = sum % 11;
            if (control == 10) control = 0;
            if (control != raw[9] - '0')
                throw new ArgumentException("Invalid EGN checksum.");

            var year = int.Parse(raw[..2]);
            var month = int.Parse(raw.Substring(2, 2));
            var day = int.Parse(raw.Substring(4, 2));

            if (month >= 1 && month <= 12)
                year += 1900;
            else if (month >= 21 && month <= 32)
            {
                year += 1800;
                month -= 20;
            }
            else if (month >= 41 && month <= 52)
            {
                year += 2000;
                month -= 40;
            }
            else
                throw new ArgumentException("Invalid EGN month.");

            DateOnly birthDate;
            try
            {
                birthDate = new DateOnly(year, month, day);
            }
            catch
            {
                throw new ArgumentException("Invalid EGN date.");
            }   

            int genderDigit = raw[8] - '0';
            var gender = (genderDigit % 2 == 0) ? Gender.Male : Gender.Female;

            return new EGN(raw, birthDate, gender);
        }

    }
}
