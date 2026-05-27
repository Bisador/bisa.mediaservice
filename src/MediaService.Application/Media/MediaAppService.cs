using MediaService.Application.Abstractions;
using MediaService.Application.DTOs;
using MediaService.Domain.Entities;

namespace MediaService.Application.Media;

public sealed class MediaAppService(IMediaRepository repository, IFileStorage fileStorage) : IMediaAppService
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "application/pdf",
        "video/mp4"
    ];

    private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB

    public async Task<Result<MediaResponse>> UploadAsync(IFormFile file, string? ownerId,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
            return Result.Failure<MediaResponse>(new FileIsEmpty());

        if (!AllowedContentTypes.Contains(file.ContentType))
            return Result.Failure<MediaResponse>(new ContentTypeIsNotAllowed(file.ContentType));

        if (file.Length > MaxFileSize)
            return Result.Failure<MediaResponse>(new FileSizeExceeded());


        await using var stream = file.OpenReadStream();

        var stored = await fileStorage.SaveAsync(
            stream,
            file.FileName,
            file.ContentType,
            cancellationToken);

        var media = new MediaItem(
            originalFileName: file.FileName,
            storedFileName: stored.StoredFileName,
            contentType: file.ContentType,
            size: file.Length,
            storagePath: stored.StoragePath,
            ownerId: ownerId);

        await repository.AddAsync(media, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(new MediaResponse(
            media.Id,
            media.OriginalFileName,
            media.ContentType,
            media.Size,
            stored.PublicUrl,
            media.CreatedAt));
    }

    public async Task<Result<MediaResult>> DownloadAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(id, cancellationToken);
        if (media is null || media.DeletedAt is not null)
            return Result.Failure<MediaResult>(new MediaNotFound());

        var stream = await fileStorage.OpenReadAsync(media.StoragePath, cancellationToken);
        return Result.Success(new MediaResult(stream, media.ContentType, media.OriginalFileName));
    }

    public async Task<Result<MediaResult>> DownloadAsync(string url, CancellationToken cancellationToken = default)
    {
        return Result.Failure<MediaResult>(new MediaNotFound());
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(id, cancellationToken);
        if (media is null || media.DeletedAt is not null)
            return Result.Failure<MediaResult>(new MediaNotFound());

        await fileStorage.DeleteAsync(media.StoragePath, cancellationToken);
        media.MarkDeleted();

        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}