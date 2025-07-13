using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Services.captcha
{
    public interface ICaptchaService
    {
        Task<Captcha> GenerateCaptchaAsync();
        Task<bool> ValidateCaptcha(Captcha captcha);
    }
}
