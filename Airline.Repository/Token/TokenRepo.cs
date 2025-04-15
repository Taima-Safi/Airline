using Airline.Database.Context;
using Airline.Database.Model;
using Airline.Dto.Token;
using Airline.Shared.Exception;
using Airline.Shared.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Airline.Repository.Token;

public class TokenRepo : ServiceRepo, ITokenRepe
{
    private readonly AirlineDbContext context;
    //private readonly IDbRepo dbRepo;
    private string Key { get; set; }
    private string Issuer { get; set; }
    private string Audience { get; set; }
    private string DurationInHours { get; set; }

    public TokenRepo(AirlineDbContext context, string durationInHours, string audience, string issuer, string key/*, IDbRepo dbRepo*/,
                IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        : base(configuration, httpContextAccessor)

    {
        //this.dbRepo = dbRepo;
        this.context = context;
        Key = this.configuration["JwtConfig:secret"];
        Issuer = this.configuration["JwtConfig:validIssuer"];
        Audience = this.configuration["JwtConfig:validAudience"];
        DurationInHours = this.configuration["JwtConfig:durationInHours"];
    }
    public async Task AddAsync(RefreshTokenDto tokenDto)
    {
        var tokenModel = new TokenModel()
        {
            JwtId = tokenDto.JwtId,
            OldJwtId = tokenDto.OldJwtId,
            UserId = tokenDto.UserId,
            RefreshToken = tokenDto.RefreshToken,
            RefreshTokenExpiryDate = tokenDto.RefreshTokenExpiryDate,
        };
        await context.AddAsync(tokenModel);
        await context.SaveChangesAsync();
    }
    public async Task<TokenDto> CreateAsync(long userId, string userType = null, string oldJwtId = null, bool? userSameToken = null, string oldRefreshToken = null)
    {
        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Jti, userSameToken.HasValue ? oldJwtId : Guid.NewGuid().ToString()),
        new(ClaimTypes.NameIdentifier, userId.ToString()),
        new(ClaimTypes.Role, userType),
        new("userType", userType)
    };
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: Issuer, audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMonths(1), // Ensure this line is correctly setting the expiration,
            signingCredentials: signingCredentials);

        var tokenDto = new TokenDto()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            RefreshToken = userSameToken.HasValue ? oldRefreshToken : StringHelper.GenerateRandomString(),
            ExpireDate = jwtSecurityToken.ValidTo,
        };

        tokenDto.Token = "Bearer " + tokenDto.Token;
        if (!userSameToken.HasValue)
            await AddAsync(new RefreshTokenDto { RefreshToken = tokenDto.RefreshToken, JwtId = jwtSecurityToken.Id, UserId = userId, OldJwtId = oldJwtId });

        return tokenDto;
    }
    public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
    {
        await context.Database.BeginTransactionAsync();
        try
        {
            TokenDto newToken = null;
            //This case mean he didn't send the token in the header.
            var bearerToken = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(bearerToken))
                throw new ValidationException("MissTokenInHeader");

            //Check if the token still work so there is no need to refresh it.
            if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                throw new SecurityTokenValidationException();

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(bearerToken.ToString().Split("Bearer ").ElementAt(1));
            var userId = jwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            var jwtId = jwtSecurityToken.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Jti).FirstOrDefault().Value;
            var userType = jwtSecurityToken.Claims.Where(c => c.Type == "userType").FirstOrDefault().Value;

            var oldTokenDto = await GetAsync(jwtId) ??
                throw new NotFoundException("refresh token not found");

            if (!oldTokenDto.IsValid)
            {
                (long Id, string jwtId, string refreshToken) alreadyRefreshedToken = await GetRefreshedOneAsync(jwtId);
                if (alreadyRefreshedToken.Id == 0)
                    throw new ValidationException("refresh token expired");

                newToken = await CreateAsync(long.Parse(userId), userType, alreadyRefreshedToken.jwtId, true, alreadyRefreshedToken.refreshToken);
                await RemoveAsync(oldTokenDto.Id);
                await context.Database.CommitTransactionAsync();
                return newToken;
            }

            if (oldTokenDto.RefreshToken != refreshToken)
                throw new ValidationException("RefreshTokenNotCorrect");

            else if (oldTokenDto.RefreshTokenExpiryDate < DateTime.UtcNow)
                throw new NotFoundException("refresh token not found"); // he should go to main page, cuz the refresh token has been expired.

            newToken = await CreateAsync(long.Parse(userId), userType, jwtId);
            await RemoveAsync(oldTokenDto.Id);
            await context.Database.CommitTransactionAsync();
            return newToken;
        }
        catch (Exception)
        {
            await context.Database.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task<RefreshTokenDto> GetAsync(string JwtId)
    {
        var refreshTokenDto = await context.Token
            .Where(ut => ut.JwtId == JwtId)
            .Select(t => new RefreshTokenDto
            {
                Id = t.Id,
                JwtId = t.JwtId,
                UserId = t.UserId,
                IsValid = t.IsValid,
                RefreshToken = t.RefreshToken,
                RefreshTokenExpiryDate = t.RefreshTokenExpiryDate,
            }).FirstOrDefaultAsync();
        return refreshTokenDto;
    }

    public async Task<(long Id, string jwtId, string refreshToken)> GetRefreshedOneAsync(string oldJwtId)
    => await context.Token
    .Where(ut => ut.IsValid && ut.OldJwtId == oldJwtId)
    .Select(ut => new Tuple<long, string, string>(ut.Id, ut.JwtId, ut.RefreshToken).ToValueTuple())
    .FirstOrDefaultAsync();

    public async Task RemoveAsync(long id) => await context.Token.Where(t => t.IsValid && t.Id == id)
    .ExecuteUpdateAsync(t => t.SetProperty(t => t.IsValid, false));
}
