using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.DTOs;
using MyPersonalDiary.Server.Repositories;
using MyPersonalDiary.Server.Services.Email;
using MyPersonalDiary.Server.Services.invite;

namespace MyPersonalDiary.Server.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IInviteService _inviteService;
        private readonly IInviteRepository _inviteRepository;
        private readonly IEmailService _emailService;

        public AdminService(AppDbContext db, IConfiguration config, IInviteService inviteService, IInviteRepository inviteRepository, IEmailService emailService)
        {
            _db = db;
            _config = config;
            _inviteService = inviteService;
            _inviteRepository = inviteRepository;
            _emailService = emailService;
        }
        public async Task<ActionResult> CreateAndSendInvite(InviteSendDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email))
                return new BadRequestObjectResult("Email is not correct");

            var invite = _inviteService.InviteCreate(dto.Email);

            if (invite == null)
                return new BadRequestObjectResult("Failed to create invite.");

            _db.Invites.Add(invite);

            await _db.SaveChangesAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("My Diary", _config["Email:From"]));
            message.To.Add(MailboxAddress.Parse(dto.Email));
            message.Subject = "Your invitation to My Personal Diary";

            message.Body = new TextPart("plain")
            {
                Text = $"Your invite: https://localhost:55040/register/{invite.Token}"
            };


            bool status = await _emailService.SendMessageAsync(message);

            //Console.WriteLine($"Status Send Message {status}");

            return new OkObjectResult("Invite send.");
        }
    }
}
