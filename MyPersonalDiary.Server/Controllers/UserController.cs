using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.Services.user;

namespace MyPersonalDiary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("delete")]
        public async Task<ActionResult> DeleteUser()
        {
            return await _userService.DeleteUserAsync();
        }
        [HttpPost("restore")]
        public async Task<ActionResult> RestoreUser()
        {
            return await _userService.RestoreUserAsync();
        }
        [HttpGet("deletion-status")]
        public async Task<ActionResult> DeletionStatus()
        {
            return await _userService.DeletionStatus();
        }

        [HttpGet("exists")]
        public async Task<ActionResult> Exists()
        {
            return await _userService.CheckExistence();
        }
    }
}
