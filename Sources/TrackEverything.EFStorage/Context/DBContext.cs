using Microsoft.EntityFrameworkCore;
using TrackEverything.EFStorage.Entities;
using TrackEverything.Storage.Core.Infrastructure;

namespace TrackEverything.EFStorage.Context
{
    /// <summary>
    /// Database Context class for working with Entity Framework
    /// </summary>
    public class DatabaseContext : DbContext
    {
        public DbSet<EFProject> Projects { get; set; }
        public DbSet<EFTask> Tasks { get; set; }
        public DbSet<EFWorker> Workers { get; set; }
        public DbSet<EFTaskWorker> TaskWorkers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(new SQLDataAccess().ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EFTask>()
                .HasOne(s => s.Project)
                .WithMany(g => g.Tasks);

            modelBuilder.Entity<EFProject>()
                .HasMany(k => k.Tasks)
                .WithOne(g => g.Project)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EFTaskWorker>()
                .HasKey(pc => new {pc.TaskId, pc.WorkerId});
            modelBuilder.Entity<EFTaskWorker>()
                .HasOne(k => k.Task)
                .WithMany(g => g.TaskWorkers)
                .HasForeignKey(k => k.TaskId);
            modelBuilder.Entity<EFTaskWorker>()
                .HasOne(k => k.Worker)
                .WithMany(g => g.TaskWorkers)
                .HasForeignKey(k => k.WorkerId);
        }
    }
}