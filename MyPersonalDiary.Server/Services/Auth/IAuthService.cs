using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.DTOs;

namespace MyPersonalDiary.Server.Services.Auth
{
    public interface IAuthService
    {
        Task<ActionResult> GetCaptcha();
        Task<ActionResult> Register(string inviteToken, RegisterDto dto);
        Task<ActionResult> Login(LoginDto dto);
        Task<ActionResult> Logout();
        Task<ActionResult> CheckSesion();
        Task<ActionResult> IsAdmin();
    }
}
