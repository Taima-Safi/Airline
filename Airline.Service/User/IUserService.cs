using Airline.Database.Model;
using Airline.Dto.Token;
using Airline.Dto.User;

namespace Airline.Service.User;

public interface IUserService
{
    Task<TokenDto> CreateTokenAsync(long userId, string userType = null, string oldJwtId = null, bool? userSameToken = null, string oldRefreshToken = null);
    Task<UserModel> GetModelByIdAsync(long id);
    Task<UserDto> LoginAsync(LoginDto dto);
    Task<TokenDto> RefreshTokenAsync(string refreshToken);
    Task<UserDto> RegisterAsync(UserFormDto dto);
}
