using System.Collections.Generic;

namespace MyPersonalDiary.Server.Models
{
    public class Captcha
    {
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; }
        public int Answer {  get; set; }
    }
}
