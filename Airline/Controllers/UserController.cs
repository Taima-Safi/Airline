using Airline.Dto.User;
using Airline.Service.User;
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

        [HttpPost]
        public async Task<IActionResult> AddUser(UserFormDto dto)
        {
            await userService.AddUserAsync(dto);
            return Ok();
        }
    }
}
