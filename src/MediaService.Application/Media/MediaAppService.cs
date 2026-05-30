using MediaService.Application.Abstractions;
using MediaService.Application.Commands;
using MediaService.Application.DTOs;
using MediaService.Application.Errors;
using MediaService.Domain.Entities;

namespace MediaService.Application.Media;

public sealed class MediaAppService(IMediaRepository repository, IFileStorage storage) : IMediaAppService
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

    public async Task<Result<MediaResponse>> UploadPersonalAsync(
        UploadPersonalMediaCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!IsValid(command.File, out var failure))
            return failure;

        await using var stream = command.File.OpenReadStream();

        var stored = await storage.SaveAsync(
            stream,
            command.File.FileName,
            command.File.ContentType,
            cancellationToken);

        var media = MediaItem.CreatePersonal(
            command.File.FileName,
            stored.BucketName,
            stored.ObjectKey,
            command.File.ContentType,
            command.File.Length,
            storage.ProviderName,
            command.OwnerId);

        try
        {
            await repository.AddAsync(media, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success(Map(media, stored.PublicUrl));
        }
        catch
        {
            await storage.DeleteAsync(
                stored.BucketName,
                stored.ObjectKey,
                cancellationToken);
            throw;
        }
    }

    public async Task<Result<MediaResponse>> UploadAttachmentAsync(
        UploadAttachmentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!IsValid(command.File, out var failure))
            return failure;

        await using var stream = command.File.OpenReadStream();

        var stored = await storage.SaveAsync(
            stream,
            command.File.FileName,
            command.File.ContentType,
            cancellationToken);

        var media = MediaItem.CreateAttachment(
            command.File.FileName,
            stored.BucketName,
            stored.ObjectKey,
            command.File.ContentType,
            command.File.Length,
            storage.ProviderName,
            command.Owner,
            command.OwnerId);

        try
        {
            await repository.AddAsync(media, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success(Map(media, stored.PublicUrl));
        }
        catch
        {
            await storage.DeleteAsync(
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

    public async Task<Result> LinkAsync(
        LinkMediaCommand command,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(command.MediaId, cancellationToken);

        if (media is null || media.IsDeleted())
            return Result.Failure(new MediaNotFound());

        media.AddLink(command.Owner);

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveLinkAsync(
        RemoveMediaLinkCommand command,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(command.MediaId, cancellationToken);

        if (media is null || media.IsDeleted())
            return Result.Failure(new MediaNotFound());

        var link = media.RemoveLink(command.LinkId);

        if (link is null)
            return Result.Failure(new MediaLinkNotFound());

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<MediaDownloadResult>> DownloadAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(id, cancellationToken);

        if (media is null || media.IsDeleted())
            return Result.Failure<MediaDownloadResult>(new MediaNotFound());

        var stream = await storage.OpenReadAsync(
            media.BucketName,
            media.ObjectKey,
            cancellationToken);

        return Result.Success(
            new MediaDownloadResult(
                stream,
                media.ContentType,
                media.OriginalFileName));
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(id, cancellationToken);

        if (media is null || media.IsDeleted())
            return Result.Failure(new MediaNotFound());

        media.MarkDeleting();
        await repository.SaveChangesAsync(cancellationToken);

        try
        {
            await storage.DeleteAsync(media.BucketName, media.ObjectKey, cancellationToken);

            media.MarkDeleted();
            await repository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            media.MarkAvailable();
            await repository.SaveChangesAsync(cancellationToken);

            throw;
        }
    }

    public async Task<Result<MediaValidationResponse>> ValidateAsync(
        ValidateMediaCommand command,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdsAsync(
            command.MediaIds,
            cancellationToken);

        var validIds = media
            .Where(x => !x.IsDeleted())
            .Select(x => x.Id)
            .ToHashSet();

        var invalidIds = command.MediaIds
            .Where(x => !validIds.Contains(x))
            .ToList();

        return Result.Success(
            new MediaValidationResponse(
                validIds.ToList(),
                invalidIds));
    }

    private static MediaResponse Map(MediaItem media, string url)
    {
        return new MediaResponse(
            media.Id,
            media.OriginalFileName,
            media.ContentType,
            media.Size,
            url,
            media.CreatedAt);
    }
}