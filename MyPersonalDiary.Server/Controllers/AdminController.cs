using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.DTOs;
using MyPersonalDiary.Server.Services.Admin;

namespace MyPersonalDiary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("invites/create-and-send")]
        public async Task<ActionResult> CreateAndSendInvite(InviteSendDto dto)
        {
            return await _adminService.CreateAndSendInvite(dto);
        }
    }
}
