using MediaService.Application.Abstractions;
using MediaService.Domain.Entities;
using MediaService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediaService.Infrastructure.Repositories;

public sealed class MediaRepository(MediaDbContext db) : IMediaRepository
{
    public async Task AddAsync(MediaItem item, CancellationToken cancellationToken = default)
        => await db.MediaItems
            .AddAsync(item, cancellationToken);

    public Task<MediaItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => db.MediaItems
            .Include(x => x.Links)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<MediaItem?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return db.MediaItems
            .SingleOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
    }

    public Task<List<MediaItem>> GetByIdsAsync(IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
        => db.MediaItems
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => db.SaveChangesAsync(cancellationToken);
}