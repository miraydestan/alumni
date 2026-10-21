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
    public DbSet<Application> Applications => Set<Application>();

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

        // Configure Application entity
        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("applications");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).HasColumnName("id");
            entity.Property(a => a.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(a => a.JobPostingId).HasColumnName("job_posting_id").IsRequired();
            entity.Property(a => a.Status).HasColumnName("status").IsRequired();

            entity.HasIndex(a => new { a.UserId, a.JobPostingId }).IsUnique();

            entity.HasOne(a => a.User)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.JobPosting)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
