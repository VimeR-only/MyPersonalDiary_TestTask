namespace MyPersonalDiary.Server.DTOs
{
    public class RecordUpdateDto
    {
        public string content { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
