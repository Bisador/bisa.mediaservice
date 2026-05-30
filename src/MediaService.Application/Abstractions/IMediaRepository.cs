using MediaService.Domain.Entities;

namespace MediaService.Application.Abstractions;

public interface IMediaRepository
{
    Task AddAsync(MediaItem item, CancellationToken cancellationToken = default);
    Task<MediaItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<MediaItem>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}