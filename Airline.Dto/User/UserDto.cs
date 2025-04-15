using Airline.Dto.Token;
using Airline.Shared.Enum;

namespace Airline.Dto.User;

public class UserDto
{
    public long Id { get; set; }
    public UserType Type { get; set; }
    public string Email { get; set; }
    public TokenDto Token { get; set; }
}
