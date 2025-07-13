using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.DTOs;

namespace MyPersonalDiary.Server.Services.Admin
{
    public interface IAdminService
    {
        Task<ActionResult> CreateAndSendInvite(InviteSendDto dto);
    }
}
