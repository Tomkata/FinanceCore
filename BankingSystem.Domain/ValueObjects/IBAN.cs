using System.Numerics;

public record IBAN
{
    public static readonly Dictionary<char, int> IbanLetterValues = new()
    {
        { 'A', 10 }, { 'B', 11 }, { 'C', 12 }, { 'D', 13 }, { 'E', 14 },
        { 'F', 15 }, { 'G', 16 }, { 'H', 17 }, { 'I', 18 }, { 'J', 19 },
        { 'K', 20 }, { 'L', 21 }, { 'M', 22 }, { 'N', 23 }, { 'O', 24 },
        { 'P', 25 }, { 'Q', 26 }, { 'R', 27 }, { 'S', 28 }, { 'T', 29 },
        { 'U', 30 }, { 'V', 31 }, { 'W', 32 }, { 'X', 33 }, { 'Y', 34 },
        { 'Z', 35 }
    };

    public string Value { get; }
    public string CountryCode { get; }
    public string CheckDigits { get; }
    public string BankCode { get; }
    public string AccountNumber { get; }

    private IBAN(string value, string countryCode, string checkDigits, string bankCode, string accountNumber)
    {
        Value = value;
        CountryCode = countryCode;
        CheckDigits = checkDigits;
        BankCode = bankCode;
        AccountNumber = accountNumber;
    }

    public static IBAN Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ArgumentException("IBAN cannot be empty.");

        raw = raw.Replace(" ", "").ToUpper();

        if (!raw.All(char.IsLetterOrDigit))
            throw new ArgumentException("IBAN must consist only of letters and digits.");

        if (raw.StartsWith("BG") && raw.Length != 22)
            throw new ArgumentException("Bulgarian IBAN must be exactly 22 characters long.");

        var rearranged = raw.Substring(4) + raw.Substring(0, 4);

        var sb = new System.Text.StringBuilder();
        foreach (var c in rearranged)
            sb.Append(char.IsLetter(c) ? IbanLetterValues[c] : c);

        BigInteger number = BigInteger.Parse(sb.ToString());

        if (number % 97 != 1)
            throw new ArgumentException("Invalid IBAN checksum.");

        string countryCode = raw[..2];
        string checkDigits = raw.Substring(2, 2);
        string bankCode = raw.Substring(4, 4);
        string accountNumber = raw.Substring(8);

        return new IBAN(raw, countryCode, checkDigits, bankCode, accountNumber);
    }
}
