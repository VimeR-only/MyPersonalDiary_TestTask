using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.Models;
using MyPersonalDiary.Server.DTOs;
using MyPersonalDiary.Server.Repositories;
using System.Threading.Tasks;
using MyPersonalDiary.Server.Services.Auth;

namespace MyPersonalDiary.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("get-captcha")]
        public async Task<ActionResult<CaptchaGetDto>> GetCaptcha()
        {
            return await _authService.GetCaptcha();
        }

        [HttpPost("register/{inviteToken}")]
        public async Task<IActionResult> Register(string inviteToken, RegisterDto dto)
        {
            return await _authService.Register(inviteToken, dto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            return await _authService.Login(dto);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return await _authService.Logout();
        }

        [HttpPost("check-session")]
        public async Task<IActionResult> CheckSesion()
        {
            return await _authService.CheckSesion();
        }

        [HttpGet("is-admin")]
        public async Task<IActionResult> IsAdmin()
        {
            return await _authService.IsAdmin();
        }
    }
}
