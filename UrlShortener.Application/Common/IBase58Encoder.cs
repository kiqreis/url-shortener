namespace UrlShortener.Application.Common;

public interface IBase58Encoder
{
    string Encode(long number);
    long Decode(string encoded);
}