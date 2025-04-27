using Airline.Database.Model;
using Airline.Dto.Token;
using Airline.Dto.User;
using Airline.Repository;
using Airline.Repository.Token;
using Airline.Shared.Enum;
using Airline.Shared.Exception;
using Airline.Shared.Helper;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Airline.Service.User;

public class UserService : IUserService
{
    private readonly IBaseRepo<UserModel> userBaseRepo;
    private readonly ITokenRepe tokenRepe;

    public UserService(IBaseRepo<UserModel> userBaseRepo, ITokenRepe tokenRepe)
    {
        this.userBaseRepo = userBaseRepo;
        this.tokenRepe = tokenRepe;
    }

    public async Task<UserDto> RegisterAsync(UserFormDto dto)
    {
        if (await userBaseRepo.CheckIfExistAsync(u => u.Email == dto.Email.Trim().ToLower() && u.IsValid))
            throw new NotFoundException("This email used before..");

        var hashPassword = PasswordHelper.HashPassword(dto.Password);

        await userBaseRepo.AddAsync(new UserModel
        {
            Email = dto.Email,
            Type = UserType.Admin,
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            HashPassword = hashPassword,
            PhoneNumber = dto.PhonNumber,
            PassportNumber = dto.PassportNumber,
            CodePhoneNumber = dto.CodePhoneNumber,
        });

        var user = await userBaseRepo.GetByAsync(x => x.Email == dto.Email);
        return new UserDto
        {
            Id = user.Id,
            Type = user.Type,
            Email = user.Email
        };
    }
    public async Task<UserDto> LoginAsync(LoginDto dto)
    {
        // var model = await context.User.Where(u => u.Email == dto.Email.Trim().ToLower() && u.IsValid).FirstOrDefaultAsync();
        var model = await userBaseRepo.GetByAsync(u => u.Email == dto.Email.Trim().ToLower() && u.IsValid);
        if (model == null)
            throw new NotFoundException("This email not found");
        await CheckPasswordCorrectness(dto.Password, model.HashPassword, model.Id);
        return new UserDto
        {
            Id = model.Id,
            Email = model.Email
        };

    }

    public async Task<TokenDto> CreateTokenAsync(long userId, string userType = null, string oldJwtId = null, bool? userSameToken = null, string oldRefreshToken = null)
    {
        return await tokenRepe.CreateAsync(userId, userType, oldJwtId, userSameToken, oldRefreshToken);
    }
    public async Task CheckPasswordCorrectness(string password, string hashPassword, long userId)
    {
        if (hashPassword == null) { throw new ValidationException("PasswordHasBeenRemoved"); }

        var isMatchPassword = PasswordHelper.CheckPassword(password, hashPassword, out string newHash);
        if (isMatchPassword == PasswordVerificationResult.Failed)
            throw new ValidationException("PasswordOrEmailWrong");

        if (isMatchPassword == PasswordVerificationResult.SuccessRehashNeeded)
            await ChangePasswordAsync(newHash, userId);
    }

    public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
    {
        return await tokenRepe.RefreshTokenAsync(refreshToken);
    }

    public async Task ChangePasswordAsync(string newHashPassword, long id)
    => await userBaseRepo.UpdateAsync(u => u.IsValid && u.Id == id, u => u.SetProperty(u => u.HashPassword, newHashPassword));
    public async Task<UserModel> GetModelByIdAsync(long id)
    {
        var user = await userBaseRepo.GetByAsync(x => x.Id == id && x.IsValid);

        return user;
    }
}
