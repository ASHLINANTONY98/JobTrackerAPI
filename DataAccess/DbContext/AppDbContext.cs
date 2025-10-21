using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DbContext
{
    namespace DataAccess.DbContext
    {
        public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<User> Users { get; set; }
            public DbSet<JobApplication> JobApplications { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<User>()
                    .HasMany(u => u.JobApplications)
                    .WithOne(j => j.User)
                    .HasForeignKey(j => j.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                base.OnModelCreating(modelBuilder);
            }
        }
    }
}