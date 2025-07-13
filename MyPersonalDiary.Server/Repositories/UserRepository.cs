using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _db.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByUserNameAsync(string userName)
        {
            return await _db.Users.AnyAsync(u => u.UserName == userName);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByPublicIdAsync(string publicId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.PublicId == publicId);
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}
