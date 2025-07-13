using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace MyPersonalDiary.Server.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<bool> SendMessageAsync(MimeMessage message)
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_config["Email:Smtp"], int.Parse(_config["Email:Port"]), true);
            await client.AuthenticateAsync(_config["Email:From"], _config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
    }
}
