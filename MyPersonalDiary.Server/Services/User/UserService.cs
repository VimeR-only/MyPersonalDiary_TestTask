using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.DTOs;
using MyPersonalDiary.Server.Models;
using MyPersonalDiary.Server.Repositories;

namespace MyPersonalDiary.Server.Services.user
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;
        private HttpContext? HttpContext => _httpContext.HttpContext;

        public UserService(AppDbContext db, IHttpContextAccessor httpContext, IUserRepository userRepository)
        {
            _db = db;
            _httpContext = httpContext;
            _userRepository = userRepository;
        }
        public User? UserCreate(string userName, string email, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            return new User {
                UserName = userName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            };
        }

        public async Task<ActionResult> DeleteUserAsync()
        {
            if (HttpContext?.Session == null || HttpContext.Session.GetString("PublicId") == null)
                return new BadRequestObjectResult(new { Message = "Session is not available." });

            var publicId = HttpContext.Session.GetString("PublicId");

            if (HttpContext.Session.GetString("PublicId") != publicId)
                return new BadRequestObjectResult(new { Message = "The transmitted ID does not match yours." });

            var user = await _userRepository.GetUserByPublicIdAsync(publicId);

            if (user == null || user.IsDeleted)
                return new BadRequestResult();

            user.IsDeleted = true;
            user.DeletionRequestedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<ActionResult> RestoreUserAsync()
        {
            if (HttpContext?.Session == null || HttpContext.Session.GetString("PublicId") == null)
                return new BadRequestObjectResult(new { Message = "Session is not available." });

            var publicId = HttpContext.Session.GetString("PublicId");

            if (HttpContext.Session.GetString("PublicId") != publicId)
                return new BadRequestObjectResult(new { Message = "The transmitted ID does not match yours." });

            var user = await _userRepository.GetUserByPublicIdAsync(publicId);

            if (user == null || !user.IsDeleted)
                return new BadRequestResult();

            user.IsDeleted = false;
            user.DeletionRequestedAt = null;

            await _db.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<ActionResult> DeletionStatus()
        {
            if (HttpContext?.Session == null || string.IsNullOrEmpty(HttpContext.Session.GetString("PublicId")) == null)
                return new BadRequestObjectResult(new { Message = "Session is not available." });

            var user = await _userRepository.GetUserByPublicIdAsync(HttpContext.Session.GetString("PublicId"));

            if (user == null)
                return new BadRequestObjectResult(new { Message = "User not found." });

            return new OkObjectResult(new { isDeleted = user.IsDeleted });
        }
        public async Task<ActionResult> CheckExistence()
        {
            var publicId = _httpContext.HttpContext?.Session?.GetString("PublicId");

            if (string.IsNullOrEmpty(publicId))
                return new OkObjectResult(new { Exists = false });

            var user = await _userRepository.GetUserByPublicIdAsync(publicId);

            return new OkObjectResult(new { Exists = user != null });
        }
    }
}
