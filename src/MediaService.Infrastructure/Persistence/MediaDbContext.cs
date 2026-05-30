using MediaService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaService.Infrastructure.Persistence;

public sealed class MediaDbContext(DbContextOptions<MediaDbContext> options) : DbContext(options)
{ 
    public DbSet<MediaItem> MediaItems => Set<MediaItem>(); 
    public DbSet<MediaLink> MediaLinks => Set<MediaLink>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediaDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}