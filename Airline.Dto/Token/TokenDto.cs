namespace Airline.Dto.Token;

public class TokenDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireDate { get; set; }
    public DateTime RefreshTokenExpireDate => DateTime.UtcNow.AddDays(90);
}
