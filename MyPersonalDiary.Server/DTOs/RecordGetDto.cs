using System.ComponentModel.DataAnnotations;

namespace MyPersonalDiary.Server.DTOs
{
    public class RecordGetDto
    {
        public string PublicId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
