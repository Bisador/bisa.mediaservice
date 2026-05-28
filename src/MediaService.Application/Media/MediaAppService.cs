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

    public async Task<Result<MediaResponse>> UploadAsync(
        IFormFile file,
        string? ownerId,
        CancellationToken cancellationToken = default)
    {
        if (!IsValid(file, out var failure))
            return failure;
 
        await using var stream = file.OpenReadStream();

        var stored = await fileStorage.SaveAsync(
            stream,
            file.FileName,
            file.ContentType,
            cancellationToken);

        var media = new MediaItem(
            originalFileName: file.FileName,
            bucketName: stored.BucketName,
            objectKey: stored.ObjectKey,
            contentType: file.ContentType,
            size: file.Length,
            storageProvider: "MinIO",
            ownerId: ownerId);

        try
        {
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
        catch (Exception e)
        {
            await fileStorage.DeleteAsync(
                stored.BucketName,
                stored.ObjectKey,
                cancellationToken);
            throw;
        }
    }

    private static bool IsValid(IFormFile file, out Result<MediaResponse> failure)
    {
        if (file.Length == 0)
        {
            failure = Result.Failure<MediaResponse>(new FileIsEmpty());
            return false;
        }

        if (string.IsNullOrWhiteSpace(file.ContentType) || !AllowedContentTypes.Contains(file.ContentType))
        {
            failure = Result.Failure<MediaResponse>(new ContentTypeIsNotAllowed(file.ContentType));
            return false;
        }

        if (file.Length > MaxFileSize)
        {
            failure = Result.Failure<MediaResponse>(new FileSizeExceeded());
            return false;
        }

        if (file.FileName.Contains("..") || Path.GetFileName(file.FileName) != file.FileName)
        {
            failure = Result.Failure<MediaResponse>(new InvalidFileName());
            return false;
        }

        failure = null!;
        return true;
    }

    public async Task<Result<MediaResult>> DownloadAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(id, cancellationToken);
        if (media is null || media.IsDeleted())
            return Result.Failure<MediaResult>(new MediaNotFound());

        var stream = await fileStorage.OpenReadAsync(
            media.BucketName,
            media.ObjectKey,
            cancellationToken);

        return Result.Success(new MediaResult(stream, media.ContentType, media.OriginalFileName));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(id, cancellationToken);
        if (media is null || media.DeletedAt is not null)
            return Result.Failure<MediaResult>(new MediaNotFound());

        media.MarkDeleting();
        await repository.SaveChangesAsync(cancellationToken);

        await fileStorage.DeleteAsync(
            media.BucketName,
            media.ObjectKey,
            cancellationToken);

        media.MarkDeleted();
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}