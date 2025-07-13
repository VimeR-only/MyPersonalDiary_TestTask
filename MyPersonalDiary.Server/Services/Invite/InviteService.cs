using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Services.invite
{
    public class InviteService : IInviteService
    {
        private readonly AppDbContext _db;

        public InviteService(AppDbContext db)
        {
            _db = db;
        }

        public Invite? InviteCreate(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return new Invite { Email = email };
        }

        public async Task<bool> IsInviteByEmailValidAsync(string email)
        {
            return await _db.Invites.AnyAsync(i => i.Email == email && !i.IsUsed);
        }

        public async Task<bool> IsInviteByTokenValidAsync(string token)
        {
            return await _db.Invites.AnyAsync(i => i.Token == token && !i.IsUsed);
        }
    }
}
