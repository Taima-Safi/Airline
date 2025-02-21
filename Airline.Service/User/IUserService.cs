using Airline.Dto.User;

namespace Airline.Service.User;

public interface IUserService
{
    Task AddUserAsync(UserFormDto dto);
}
