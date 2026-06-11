using MediaService.Application.Abstractions;
using MediaService.Application.Commands;
using MediaService.Application.DTOs;
using MediaService.Application.Errors;
using MediaService.Domain.Entities;
using MediaService.Domain.Enums;

namespace MediaService.Application.Services;

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

        var objectKey =
            ObjectKeyGenerator.Generate(command.TenantId, MediaItem.PersonalCategory, command.File.FileName);


        var stored = await storage.SaveAsync(
            objectKey,
            stream,
            command.File.FileName,
            command.File.ContentType,
            cancellationToken);

        var media = MediaItem.CreatePersonal(
            command.TenantId,
            command.File.FileName,
            stored.BucketName,
            stored.ObjectKey,
            command.File.ContentType,
            command.File.Length,
            storage.ProviderName,
            command.OwnerId,
            command.AccessLevel);

        try
        {
            await repository.AddAsync(media, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success(Map(
                category: MediaItem.PersonalCategory,
                accessLevel: media.AccessLevel,
                media: media,
                url: stored.PublicUrl));
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

        var objectKey = ObjectKeyGenerator.Generate(command.TenantId, command.Category, command.File.FileName);

        var stored = await storage.SaveAsync(
            objectKey,
            stream,
            command.File.FileName,
            command.File.ContentType,
            cancellationToken);

        var media = MediaItem.CreateAttachment(
            command.TenantId,
            command.File.FileName,
            stored.BucketName,
            command.Category,
            stored.ObjectKey,
            command.File.ContentType,
            command.File.Length,
            storage.ProviderName,
            command.Owner,
            command.OwnerId,
            command.AccessLevel);

        try
        {
            await repository.AddAsync(media, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success(Map(
                category: command.Category,
                accessLevel: media.AccessLevel,
                media: media,
                url: stored.PublicUrl));
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

    public async Task<Result<List<MediaResponse>>> UploadAttachmentBatchAsync(
        UploadAttachmentBatchCommand command,
        CancellationToken cancellationToken = default)
    {
        foreach (var commandFile in command.Files)
        {
            if (!IsValid(commandFile, out var failure))
                return Result.Failure<List<MediaResponse>>(failure.Error);
        }

        List<(StoredFileResult Stored, MediaItem Media)> insertedItems = [];
        try
        {
            foreach (var commandFile in command.Files)
            {
                await using var stream = commandFile.OpenReadStream();
                var objectKey = ObjectKeyGenerator.Generate(command.TenantId, command.Category, commandFile.FileName);

                var stored = await storage.SaveAsync(
                    objectKey,
                    stream,
                    commandFile.FileName,
                    commandFile.ContentType,
                    cancellationToken);

                var media = MediaItem.CreateAttachment(
                    command.TenantId,
                    commandFile.FileName,
                    stored.BucketName,
                    command.Category,
                    stored.ObjectKey,
                    commandFile.ContentType,
                    commandFile.Length,
                    storage.ProviderName,
                    command.Owner,
                    command.OwnerId,
                    command.AccessLevel);

                insertedItems.Add((stored, media));

                await repository.AddAsync(media, cancellationToken);
            }

            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success(insertedItems.Select(item => Map(
                category: command.Category,
                accessLevel: item.Media.AccessLevel,
                media: item.Media,
                url: item.Stored.PublicUrl)).ToList());
        }
        catch
        {
            foreach (var storedFileResult in insertedItems)
            {
                await storage.DeleteAsync(
                    storedFileResult.Media.BucketName,
                    storedFileResult.Media.ObjectKey,
                    cancellationToken);
            }

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
            failure = Result.Failure<MediaResponse>(new FileSizeExceededError());
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
 
        if (!IsMediaExisted(media, command.TenantId))
            return Result.Failure(new MediaNotFoundError());

        media!.AddLink(command.Owner);

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveLinkAsync(
        RemoveMediaLinkCommand command,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(command.MediaId, cancellationToken);
  
        if (!IsMediaExisted(media, command.TenantId))
            return Result.Failure(new MediaNotFoundError());

        if (media!.Purpose == MediaPurpose.Attachment)
        {
            if (await DeleteMediaAsync(media, cancellationToken) is { IsFailure: true } removeResult)
                return removeResult;
            return Result.Success();
        }

        var link = media.RemoveLink(command.LinkId);
        if (link is null)
            return Result.Failure(new MediaLinkNotFound());

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveLinkByOwnerAsync(
        RemoveMediaLinkByOwnerCommand command,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(command.MediaId, cancellationToken);

        if (!IsMediaExisted(media, command.TenantId))
            return Result.Failure(new MediaNotFoundError());

        if (media!.Purpose == MediaPurpose.Attachment)
        {
            if (await DeleteMediaAsync(media, cancellationToken) is { IsFailure: true } removeResult)
                return removeResult;
            return Result.Success();
        }

        var link = media.RemoveLink(command.OwnerType, command.OwnerId);
        if (link is null)
            return Result.Failure(new MediaLinkNotFound());

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<MediaDownloadResult>> DownloadPublicAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(id, cancellationToken);

        if (media is null || media.IsDeleted())
            return Result.Failure<MediaDownloadResult>(new MediaNotFoundError());

        if (media.AccessLevel != MediaAccessLevel.Public)
        {
            return Result.Failure<MediaDownloadResult>(new AccessDeniedError());
        }

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

    public async Task<Result<MediaDownloadResult>> DownloadInternalAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(id, cancellationToken);

        if (!IsMediaExisted(media, tenantId))
            return Result.Failure<MediaDownloadResult>(new MediaNotFoundError());

        if (media!.AccessLevel != MediaAccessLevel.Public && media.TenantId != tenantId)
        {
            return Result.Failure<MediaDownloadResult>(new AccessDeniedError());
        }

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
        Guid tenantId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(id, cancellationToken);

        return IsMediaExisted(media, tenantId)
            ? await DeleteMediaAsync(media!, cancellationToken)
            : Result.Failure(new MediaNotFoundError());
    }

    private async Task<Result> DeleteMediaAsync(MediaItem media, CancellationToken cancellationToken)
    {
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
        var media = await repository.GetByIdsAsync(command.MediaIds, cancellationToken);

        var validIds = media
            .Where(x => x.TenantId == command.TenantId)
            .Where(x => !x.IsDeleted())
            .Select(x => x.Id)
            .ToHashSet();

        var invalidIds = command.MediaIds
            .Where(x => !validIds.Contains(x))
            .ToList();

        return Result.Success(new MediaValidationResponse(validIds.ToList(), invalidIds));
    }

    public async Task<Result<MediaMetadataResponse>> MetadataAsync(
        MetadataMediaCommand command,
        CancellationToken cancellationToken = default)
    {
        var media = await repository.GetByIdAsync(command.Id, cancellationToken);

        return IsMediaExisted(media, command.TenantId)
            ? Result.Success(MediaMetadataResponse.Existed(
                media!.Id,
                media.TenantId,
                media.Status,
                media.ContentType,
                media.AccessLevel,
                media.Category,
                media.Size
            ))
            : Result.Failure<MediaMetadataResponse>(new MediaNotFoundError());
    }

    public async Task<Result<List<MediaMetadataResponse>>> MetadataBatchAsync(
        MetadataBatchMediaCommand command,
        CancellationToken cancellationToken = default)
    {
        var mediaItems = await repository.GetByIdsAsync(command.Ids, cancellationToken);
        List<MediaMetadataResponse> result = [];
        foreach (var mediaItem in mediaItems)
        {
            if (IsMediaExisted(mediaItem, command.TenantId))
                result.Add(MediaMetadataResponse.Existed(
                    mediaItem.Id,
                    mediaItem.TenantId,
                    mediaItem.Status,
                    mediaItem.ContentType,
                    mediaItem.AccessLevel,
                    mediaItem.Category,
                    mediaItem.Size
                ));
            else
                result.Add(MediaMetadataResponse.NotExisted(mediaItem.Id));
        }

        return Result.Success(result);
    }

    private static MediaResponse Map(string category, MediaAccessLevel accessLevel, MediaItem media, string url)
    {
        return new MediaResponse(
            media.Id,
            category,
            accessLevel,
            media.OriginalFileName,
            media.ContentType,
            media.Size,
            url,
            media.CreatedAt);
    }

    private static bool IsMediaExisted(MediaItem? media, Guid tenantId) =>
        media is not null && !media.IsDeleted() && media.TenantId == tenantId;
}