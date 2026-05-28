using MediaService.Application.DTOs;
using MediaService.Application.Media;

namespace MediaService.Application.Abstractions;

public interface IMediaAppService
{
    Task<Result<MediaResponse>> UploadAsync(IFormFile file, string? ownerId, CancellationToken cancellationToken = default);
    Task<Result<MediaResult>> DownloadAsync(Guid id, CancellationToken cancellationToken = default); 
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}