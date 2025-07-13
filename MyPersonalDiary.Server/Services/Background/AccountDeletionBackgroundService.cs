using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Server.Data;

namespace MyPersonalDiary.Server.Services.Background
{
    public class AccountDeletionBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scope;
        //private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public AccountDeletionBackgroundService(IServiceScopeFactory scope)
        {
            _scope = scope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scope.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                //var threshold = DateTime.UtcNow.AddHours(-48);
                var threshold = DateTime.UtcNow.AddMinutes(-0.5);

                var usersToDelete = await _db.Users
                    .Where(u => u.IsDeleted && u.DeletionRequestedAt != null && u.DeletionRequestedAt <= threshold).ToListAsync(stoppingToken);

                if (usersToDelete.Any())
                {
                    _db.Users.RemoveRange(usersToDelete);

                    await _db.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
