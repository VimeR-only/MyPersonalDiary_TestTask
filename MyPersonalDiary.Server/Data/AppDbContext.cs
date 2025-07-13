using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Server.Models;

namespace MyPersonalDiary.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Invite> Invites { get; set; }
    }
}
