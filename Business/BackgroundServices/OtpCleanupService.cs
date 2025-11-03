using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.DbContext.DataAccess.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Business.BackgroundServices
{
    public class OtpCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public OtpCleanupService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[OtpCleanupService] Background service started.");

            // Run cleanup immediately on startup
            await RunCleanupAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"[OtpCleanupService] Checking for expired users at {DateTime.UtcNow}...");
                await RunCleanupAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken); // ⏱ Short delay for testing
            }
        }

        private async Task RunCleanupAsync(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cutoffTime = DateTime.UtcNow.AddMinutes(-2);
            var expiredUsers = await db.Users
                .Where(u => !u.IsVerified && u.OtpExpiry < cutoffTime)
                .ToListAsync(stoppingToken);

            if (expiredUsers.Any())
            {
                foreach (var user in expiredUsers)
                {
                    Console.WriteLine($"[Cleanup] Deleting user {user.Id} - {user.Email}, OtpExpiry: {user.OtpExpiry}");
                }

                db.Users.RemoveRange(expiredUsers);
                await db.SaveChangesAsync(stoppingToken);
                Console.WriteLine($"[Cleanup] Deleted {expiredUsers.Count} unverified users.");
            }
            else
            {
                Console.WriteLine("[Cleanup] No expired users found.");
            }
        }
    }
}
