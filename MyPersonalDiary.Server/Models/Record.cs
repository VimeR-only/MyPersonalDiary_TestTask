using System.ComponentModel.DataAnnotations;

namespace MyPersonalDiary.Server.Models
{
    public class Record
    {
        public int Id { get; set; }
        public string PublicId { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
