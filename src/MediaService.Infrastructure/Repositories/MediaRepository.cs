using MediaService.Application.Abstractions;
using MediaService.Domain.Entities;
using MediaService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediaService.Infrastructure.Repositories;

public sealed class MediaRepository(MediaDbContext db) : IMediaRepository
{
    public async Task AddAsync(MediaItem item, CancellationToken cancellationToken = default)
        => await db.MediaItems.AddAsync(item, cancellationToken);

    public Task<MediaItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => db.MediaItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => db.SaveChangesAsync(cancellationToken);
}