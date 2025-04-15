using Microsoft.AspNetCore.Identity;

namespace Airline.Shared.Helper;

public class PasswordHelper
{
    public static string HashPassword(string password)
    {
        var passwordHasher = new PasswordHasher<object>();
        return passwordHasher.HashPassword(null, password);
    }

    public static PasswordVerificationResult CheckPassword(string password, string hash, out string newHash)
    {
        var passwordHasher = new PasswordHasher<object>();
        var result = passwordHasher.VerifyHashedPassword(null, hash, password);
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            newHash = HashPassword(password);
            return result;
        }
        else if (result == PasswordVerificationResult.Success)
        {
            newHash = hash;
            return result;
        }
        newHash = null;
        return result;
    }

    //public static bool CheckPasswordPolicy(string password,/* PasswordCriteria opts = null*/)
    //{
    //    //opts ??= new PasswordCriteria();

    //    //if (password.Length < opts.RequiredLength)
    //    //    return false;

    //    int digitCount = 0, lowerCount = 0, upperCount = 0, symbolCount = 0;
    //    var set = new HashSet<char>();

    //    foreach (var character in password)
    //    {
    //        if (char.IsDigit(character))
    //            digitCount++;
    //        else if (char.IsUpper(character))
    //            upperCount++;
    //        else if (char.IsLower(character))
    //            lowerCount++;
    //        else
    //            symbolCount++;

    //        set.Add(character);
    //    }
    //    return set.Count >= opts.RequiredUniqueChars
    //        && (!opts.RequireDigit || digitCount > 0)
    //        && (!opts.RequireLowercase || lowerCount > 0)
    //        && (!opts.RequireUppercase || upperCount > 0)
    //        && (!opts.RequireNonAlphanumeric || symbolCount > 0);
    //}
}
