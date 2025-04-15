using System.Security.Cryptography;

namespace Airline.Shared.Helper;

public class StringHelper
{
    public static string GenerateRandomString()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
