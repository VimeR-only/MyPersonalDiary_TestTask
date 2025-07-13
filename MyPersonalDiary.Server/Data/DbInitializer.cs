using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Server.Models;
using MyPersonalDiary.Server.Services.user;

namespace MyPersonalDiary.Server.Data
{
    public class DbInitializer
    {
        private readonly AppDbContext _db;
        private readonly IUserService _userService;

        public DbInitializer(AppDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }

        public async Task InitializeAsync()
        {
            //_db.Invites.ExecuteDelete();

            if (!await _db.Users.AnyAsync(u => u.Role == UserRole.Admin))
                {
                var user = _userService.UserCreate("admin", "admin@email.com", "admin");

                if (user == null)
                {
                    Console.WriteLine("[!] Error create first admin user.");
                    return;
                }

                user.Role = UserRole.Admin;

                _db.Users.Add(user);

                Console.WriteLine("[!] First Admin user created.");

                await _db.SaveChangesAsync();
            }
        }
    }
}
