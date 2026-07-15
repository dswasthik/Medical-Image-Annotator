using MedImage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedImage.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Study> Studies => Set<Study>();
    public DbSet<Annotation> Annotations => Set<Annotation>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Username).HasMaxLength(64).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.PasswordSalt).IsRequired();
        });

        b.Entity<Study>(e =>
        {
            e.Property(s => s.Title).HasMaxLength(200).IsRequired();
            e.Property(s => s.OriginalImagePath).IsRequired();
            e.HasOne(s => s.User)
             .WithMany(u => u.Studies)
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Annotation>(e =>
        {
            e.Property(a => a.Type).HasConversion<string>().HasMaxLength(20);
            e.HasOne(a => a.Study)
             .WithMany(s => s.Annotations)
             .HasForeignKey(a => a.StudyId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(b);
    }
}
