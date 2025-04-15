using Airline.Dto.Token;

namespace Airline.Repository.Token;

public interface ITokenRepe
{
    Task AddAsync(RefreshTokenDto tokenDto);
    Task<TokenDto> CreateAsync(long userId, string userType = null, string oldJwtId = null, bool? userSameToken = null, string oldRefreshToken = null);
    Task<TokenDto> RefreshTokenAsync(string refreshToken);
}
