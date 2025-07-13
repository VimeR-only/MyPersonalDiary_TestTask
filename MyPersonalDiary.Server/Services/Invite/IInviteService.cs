using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Services.invite
{
    public interface IInviteService
    {
        Invite InviteCreate(string email);
        Task<bool> IsInviteByTokenValidAsync(string publicId);
        Task<bool> IsInviteByEmailValidAsync(string email);
    }
}
