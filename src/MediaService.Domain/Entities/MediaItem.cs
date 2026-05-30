using MediaService.Domain.Enums;
using MediaService.Domain.Exceptions;
using MediaService.Domain.Models;

namespace MediaService.Domain.Entities;

public class MediaItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string OriginalFileName { get; private set; } = null!;
    public string BucketName { get; private set; } = null!;
    public string ObjectKey { get; private set; } = null!;

    public string ContentType { get; private set; } = null!;
    public long Size { get; private set; }
    public string StorageProvider { get; private set; } = null!;
    public MediaStatus Status { get; private set; }

    public MediaPurpose Purpose { get; private set; }
    public ICollection<MediaLink> Links { get; private set; } = [];

    public string? OwnerId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; private set; }
    public string? Sha256 { get; private set; }

    public static MediaItem CreatePersonal(
        string originalFileName,
        string bucketName,
        string objectKey,
        string contentType,
        long size,
        string provider,
        string? ownerId)
    {
        return new MediaItem(
            originalFileName,
            bucketName,
            objectKey,
            contentType,
            size,
            provider,
            ownerId,
            MediaPurpose.PersonalStorage);
    }

    public static MediaItem CreateAttachment(
        string originalFileName,
        string bucketName,
        string objectKey,
        string contentType,
        long size,
        string provider,
        OwnerReference link,
        string? ownerId)
    {
        var media = new MediaItem(
            originalFileName,
            bucketName,
            objectKey,
            contentType,
            size,
            provider,
            ownerId,
            MediaPurpose.Attachment);

        media.AddLink(link);
        return media;
    }


    private MediaItem(
        string originalFileName,
        string bucketName,
        string objectKey,
        string contentType,
        long size,
        string storageProvider,
        string? ownerId,
        MediaPurpose purpose,
        string? sha256 = null) : this()
    {
        Id = Guid.NewGuid();

        OriginalFileName = originalFileName;

        BucketName = bucketName;
        ObjectKey = objectKey;

        ContentType = contentType;
        Size = size;

        StorageProvider = storageProvider;
        Purpose = purpose;
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