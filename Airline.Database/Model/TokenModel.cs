namespace Airline.Database.Model
{
    public class TokenModel : BaseModel
    {
        public string JwtId { get; set; }
        public string OldJwtId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDate { get; set; }

        public long UserId { get; set; }
        public UserModel User { get; set; }
    }
}
