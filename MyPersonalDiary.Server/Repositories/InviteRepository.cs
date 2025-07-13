using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.Models;
namespace MyPersonalDiary.Server.Repositories
{
    public class InviteRepository : IInviteRepository
    {
        private readonly AppDbContext _db;

        public InviteRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Invite?> GetInviteById(int id)
        {
            return await _db.Invites.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invite?> GetInviteByToken(string token)
        {
            return await _db.Invites.FirstOrDefaultAsync(i => i.Token == token);
        }

        public async Task<Invite?> GetInviteByEmail(string email)
        {
            return await _db.Invites.FirstOrDefaultAsync(i => i.Email == email);
        }
    }
}
