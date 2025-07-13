using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.DTOs;
using MyPersonalDiary.Server.Models;
using MyPersonalDiary.Server.Repositories;
using MyPersonalDiary.Server.Services.captcha;
using MyPersonalDiary.Server.Services.invite;
using MyPersonalDiary.Server.Services.user;

namespace MyPersonalDiary.Server.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContext;

        private readonly ICaptchaService _captchaService;
        private readonly IInviteService _inviteService;
        private readonly IInviteRepository _inviteRepository;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        private HttpContext? HttpContext => _httpContext.HttpContext;

        public AuthService(AppDbContext db, ICaptchaService captcha, IInviteService invite, IInviteRepository inviteRepository,IUserRepository user, IUserService userService,IHttpContextAccessor httpContext)
        {
            _db = db;
            _captchaService = captcha;
            _inviteService = invite;
            _inviteRepository = inviteRepository;
            _userRepository = user;
            _httpContext = httpContext;
            _userService = userService;
        }

        public async Task<ActionResult> GetCaptcha()
        {
            if (HttpContext?.Session == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            var captcha = await _captchaService.GenerateCaptchaAsync();

            HttpContext.Session.SetInt32("CaptchaAnswer", captcha.Answer);

            CaptchaGetDto dto = new CaptchaGetDto
            {
                FirstNumber = captcha.FirstNumber,
                SecondNumber = captcha.SecondNumber,
            };

            return new OkObjectResult(dto);
        }

        public async Task<ActionResult> Register(string inviteToken, RegisterDto dto)
        {
            if (HttpContext?.Session == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            var captchaAnswer = HttpContext.Session.GetInt32("CaptchaAnswer");

            if (!captchaAnswer.HasValue || captchaAnswer != dto.CaptchaAnswer)
                return new BadRequestObjectResult(new { Message = "Incorrect captcha." });

            bool inviteValid = await _inviteService.IsInviteByTokenValidAsync(inviteToken);
            if (!inviteValid)
                return new BadRequestObjectResult(new { Message = "The invite is not valid." });

            if (await _userRepository.ExistsByEmailAsync(dto.Email) || await _userRepository.ExistsByUserNameAsync(dto.UserName))
                return new BadRequestObjectResult(new { Message = "User already exists" });

            var invite = await _inviteRepository.GetInviteByEmail(dto.Email);

            if (invite == null)
                return new NotFoundObjectResult(new { Message = "Invite not found." });

            invite.IsUsed = true;

            var newUser = _userService.UserCreate(dto.UserName, dto.Email, dto.Password);

            if (newUser == null)
                return new OkObjectResult(new { Message = "Error Create user." });


            _db.Users.Add(newUser);
            _db.Invites.Update(invite);

            await _db.SaveChangesAsync();

            //HttpContext.Session.SetString("PublicId", user.PublicId);

            return new OkObjectResult(new { Message = "Registration successful." });
        }

        public async Task<ActionResult> Login(LoginDto dto)
        {
            if (HttpContext?.Session?.GetString("PublicId") != null)
                return new BadRequestObjectResult(new { Message = "You are already logged in." });

            if (dto == null || string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.Password))
                return new BadRequestObjectResult(new { Message = "Username and password are required" });

            var user = await _userRepository.GetUserByUserNameAsync(dto.UserName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new UnauthorizedObjectResult(new { Message = "Invalid data" });

            HttpContext.Session.SetString("PublicId", user.PublicId);

            return new OkObjectResult(new { Message = "Login successful" });
        }

        public async Task<ActionResult> Logout()
        {
            if (HttpContext?.Session?.GetString("PublicId") == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            HttpContext.Session.Clear();

            return new OkObjectResult(new { Message = "Exit successful" });
        }

        public async Task<ActionResult> CheckSesion()
        {
            if (HttpContext?.Session?.GetString("PublicId") == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            var publicId = HttpContext.Session.GetString("PublicId");

            if (publicId == null)
            {
                return new UnauthorizedObjectResult(new { Message = "Not authorized." });
            }

            var user = _userRepository.GetUserByPublicIdAsync(publicId);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Message = "User not found." });
            }

            return new OkObjectResult(new { Message = "Authorized" });
        }

        public async Task<ActionResult> IsAdmin()
        {
            if (HttpContext?.Session?.GetString("PublicId") == null)
            {
                return new BadRequestObjectResult(new { Message = "Session is not available." });
            }

            var publicId = HttpContext.Session.GetString("PublicId");
            if (publicId == null)
                return new UnauthorizedObjectResult(new { Message = "Not authorized." });

            var user = await _userRepository.GetUserByPublicIdAsync(publicId);

            if (user == null)
                return new NotFoundObjectResult(new { Message = "User not found." });

            return new OkObjectResult(new { isAdmin = user.Role.ToString() == "Admin" });
        }
    }
}
