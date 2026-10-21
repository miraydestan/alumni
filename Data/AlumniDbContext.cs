using Microsoft.EntityFrameworkCore;
using Alumni.Models;

namespace Alumni.Data;

public class AlumniDbContext : DbContext
{
    public AlumniDbContext(DbContextOptions<AlumniDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired();
            entity.Property(u => u.Email).IsRequired();
        });

        // Configure Announcement entity
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Title).IsRequired();
            entity.Property(a => a.Content).IsRequired();
            entity.Property(a => a.CreatedDate).IsRequired();
        });

        // Configure JobPosting entity
        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.ToTable("job_postings");
            entity.HasKey(j => j.Id);
            entity.Property(j => j.Id).HasColumnName("id");
            entity.Property(j => j.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(j => j.Title).HasColumnName("title").IsRequired();
            entity.Property(j => j.Company).HasColumnName("company").IsRequired();

            entity.HasOne(j => j.User)
                .WithMany(u => u.JobPostings)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
