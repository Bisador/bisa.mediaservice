using MediaService.Domain.Enums;
using MediaService.Domain.Exceptions;
using MediaService.Domain.Models;

namespace MediaService.Domain.Entities;

public class MediaItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }

    public string OriginalFileName { get; private set; } = null!;
    public string BucketName { get; private set; } = null!;
    public string ObjectKey { get; private set; } = null!;

    public string ContentType { get; private set; } = null!;
    public long Size { get; private set; }
    public string StorageProvider { get; private set; } = null!;
    public MediaStatus Status { get; private set; }

    public MediaPurpose Purpose { get; private set; }
    public ICollection<MediaLink> Links { get; private set; } = [];
    public MediaAccessLevel AccessLevel { get; private set; } = MediaAccessLevel.Private;


    public string? OwnerId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; private set; }
    public string? Sha256 { get; private set; }

    public static MediaItem CreatePersonal(
        Guid tenantId,
        string originalFileName,
        string bucketName,
        string objectKey,
        string contentType,
        long size,
        string provider,
        string? ownerId,
        MediaAccessLevel? accessLevel = null)
    {
        return new MediaItem(
            tenantId,
            originalFileName,
            bucketName,
            objectKey,
            contentType,
            size,
            provider,
            ownerId,
            MediaPurpose.PersonalStorage,
            accessLevel ?? MediaAccessLevel.Private
        );
    }

    public static MediaItem CreateAttachment(
        Guid tenantId,
        string originalFileName,
        string bucketName,
        string objectKey,
        string contentType,
        long size,
        string provider,
        OwnerReference link,
        string? ownerId,
        MediaAccessLevel? accessLevel = null)
    {
        var media = new MediaItem(
            tenantId,
            originalFileName,
            bucketName,
            objectKey,
            contentType,
            size,
            provider,
            ownerId,
            MediaPurpose.Attachment,
            accessLevel ?? MediaAccessLevel.Private);

        media.AddLink(link);
        return media;
    }


    private MediaItem(
        Guid tenantId,
        string originalFileName,
        string bucketName,
        string objectKey,
        string contentType,
        long size,
        string storageProvider,
        string? ownerId,
        MediaPurpose purpose,
        MediaAccessLevel accessLevel,
        string? sha256 = null) : this()
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;

        OriginalFileName = originalFileName;

        BucketName = bucketName;
        ObjectKey = objectKey;

        ContentType = contentType;
        Size = size;

        StorageProvider = storageProvider;
        Purpose = purpose;
        AccessLevel = accessLevel;
        OwnerId = ownerId;

        Sha256 = sha256;

        Status = MediaStatus.Available;

        CreatedAt = DateTime.UtcNow;
    }

    private MediaItem()
    {
    }


    public void MarkUploading()
    {
        Status = MediaStatus.Uploading;
    }

    public void MarkAvailable()
    {
        Status = MediaStatus.Available;
    }

    public void MarkDeleting()
    {
        if (Purpose == MediaPurpose.PersonalStorage && HasLinks)
            throw new MediaWithLinksCantBeRemovedException();

        Status = MediaStatus.Deleting;
    }

    public void MarkDeleted()
    {
        Status = MediaStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
    }

    public MediaLink AddLink(OwnerReference ownerReference)
    {
        if (!CanAcceptLink)
            throw new MediaIsNotAvailableException();

        if (Purpose == MediaPurpose.Attachment && Links.Count != 1)
            throw new AttachmentMustHaveOnlyOneLinkException();

        var link = new MediaLink(ownerReference.OwnerType, ownerReference.OwnerId);

        if (Links.Any(p => p.Equals(link)))
            throw new DuplicateLinkException();

        Links.Add(link);

        return link;
    }

    public MediaLink? RemoveLink(Guid linkId)
    {
        if (Purpose == MediaPurpose.Attachment)
            throw new AttachmentLinkCantBeRemovedException();

        var link = Links.FirstOrDefault(p => p.Id.Equals(linkId));
        if (link is null)
            return null;

        Links.Remove(link);
        return link;
    }

    public bool IsDeleted() => Status == MediaStatus.Deleted;
    public bool HasLinks => Links.Any();
    public bool CanAcceptLink => Status == MediaStatus.Available;
}