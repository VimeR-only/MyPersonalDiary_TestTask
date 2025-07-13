using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Services.captcha
{
    public class CaptchaService : ICaptchaService
    {
        public async Task<Captcha> GenerateCaptchaAsync()
        {
            var ran = new Random();
            int first = ran.Next(1, 100);
            int second = ran.Next(1, 100);
            int answer = first + second;

            Captcha Captcha = new Captcha
            {
                FirstNumber = first,
                SecondNumber = second,
                Answer = answer
            };

            return Captcha;
        }

        public async Task<bool> ValidateCaptcha(Captcha captcha)
        {
            if (captcha == null)
                return false;

            return captcha.Answer == captcha.FirstNumber + captcha.SecondNumber;
        }
    }
}
