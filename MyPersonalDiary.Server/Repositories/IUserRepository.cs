using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUserNameAsync(string userName);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByPublicIdAsync(string publicId);
        Task<User?> GetUserByUserNameAsync(string userName);
    }
}
