using MediaService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaService.Infrastructure.Persistence;

public sealed class MediaDbContext(DbContextOptions<MediaDbContext> options) : DbContext(options)
{
    public DbSet<MediaItem> MediaItems => Set<MediaItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaItem>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.OriginalFileName).HasMaxLength(512).IsRequired();
            b.Property(x => x.StoredFileName).HasMaxLength(512).IsRequired();
            b.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
            b.Property(x => x.StoragePath).HasMaxLength(1024).IsRequired();
            b.Property(x => x.OwnerId).HasMaxLength(128);

            b.HasIndex(x => x.CreatedAt);
            b.HasIndex(x => x.OwnerId);
        });
    }
}