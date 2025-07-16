namespace UrlShortener.Application.Common;

public class Base58Encoder : IBase58Encoder
{
    private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    public static readonly int Base = Alphabet.Length;

    private static readonly Dictionary<char, int> MapChar = Alphabet.Select((c, i) => new { Char = c, Index = i })
        .ToDictionary(x => x.Char, x => x.Index);

    public string Encode(long number)
    {
        switch (number)
        {
            case < 0:
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be non-negative");
            case 0:
                return Alphabet[0].ToString();
        }

        var stack = new Stack<char>();

        while (number > 0)
        {
            stack.Push(Alphabet[(int)(number % Base)]);
            number /= Base;
        }

        return new string([.. stack]);
    }

    public long Decode(string encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded))
            throw new ArgumentException("Encoded string cannot be null or empty", nameof(encoded));

        long result = 0;

        foreach (var c in encoded)
        {
            if (!MapChar.TryGetValue(c, out var digit))
                throw new ArgumentException($"Invalid character '{c}' in Base58", nameof(encoded));

            result = result * Base + digit;
        }

        return result;
    }
}