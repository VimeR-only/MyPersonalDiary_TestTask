using MimeKit;

namespace MyPersonalDiary.Server.Services.Email
{
    public interface IEmailService
    {
        Task<bool> SendMessageAsync(MimeMessage message);
    }
}
