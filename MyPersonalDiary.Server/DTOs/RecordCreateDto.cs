using System.ComponentModel.DataAnnotations;

namespace MyPersonalDiary.Server.DTOs
{
    public class RecordCreateDto
    {
        public string content {  get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
