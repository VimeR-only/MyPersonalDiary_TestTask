namespace MyPersonalDiary.Server.Models
{
    public enum UserRole
    {
        User,
        Admin
    }
    public class User
    {
        public int Id { get; set; }
        public string PublicId { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime? DeletionRequestedAt { get; set; }
    }
}
