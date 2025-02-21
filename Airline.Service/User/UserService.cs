using Airline.Database.Model;
using Airline.Dto.User;
using Airline.Repository;

namespace Airline.Service.User;

public class UserService : IUserService
{
    private readonly IBaseRepo<UserModel> userBaseRepo;

    public UserService(IBaseRepo<UserModel> userBaseRepo)
    {
        this.userBaseRepo = userBaseRepo;
    }
    public async Task AddUserAsync(UserFormDto dto)
        => await userBaseRepo.AddAsync(new UserModel
        {
            Email = dto.Email,
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            PhonNumber = dto.PhonNumber,
            PassportNumber = dto.PassportNumber,
        });

}
