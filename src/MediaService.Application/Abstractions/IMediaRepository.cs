using MediaService.Domain.Entities;

namespace MediaService.Application.Abstractions;

public interface IMediaRepository
{
    Task AddAsync(MediaItem item, CancellationToken cancellationToken = default);
    Task<MediaItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<List<MediaItem>> GetByIdsAsync(IReadOnlyCollection<Guid> commandMediaIds, CancellationToken cancellationToken);
}