using Airline.Dto.User;
using Airline.Service.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        // [HttpPost]
        // public async Task<IActionResult> AddUser(UserFormDto dto)
        // {
        //     await userService.AddUserAsync(dto);
        //     return Ok();
        // }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(UserFormDto dto)
        {
            var user = await userService.RegisterAsync(dto);
            user.Token = await userService.CreateTokenAsync(user.Id, user.Type.ToString());

            return Ok(user);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var userDto = await userService.LoginAsync(dto);
            userDto.Token = await userService.CreateTokenAsync(userDto.Id, userDto.Type.ToString());

            return Ok(userDto);
        }
    }
}
