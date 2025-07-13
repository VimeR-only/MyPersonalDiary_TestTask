using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Repositories
{
    public interface IInviteRepository
    {
        public Task<Invite?> GetInviteById(int id);
        public Task<Invite?> GetInviteByToken(string token);
        public Task<Invite?> GetInviteByEmail(string email);
    }
}
