namespace Airline.Dto.Token;

public class RefreshTokenDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string JwtId { get; set; }
    public bool IsValid { get; set; }
    public string OldJwtId { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryDate { get; set; } = DateTime.UtcNow.AddDays(90);
}
