using MediaService.Domain.Enums;

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
            MediaPurpose.Attachment);
    }
    

    private MediaItem(
        string originalFileName,
        string bucketName,
        string objectKey,
        string contentType,
        long size,
        string storageProvider,
        string? ownerId,
        MediaPurpose  purpose,
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

        Status = MediaStatus.Uploading;

        CreatedAt = DateTime.UtcNow;
    }

    private MediaItem()
    {
    }


    public void MarkAvailable()
    {
        Status = MediaStatus.Available;
    }

    public void MarkDeleting()
    {
        // todo: if last owned link is removed then soft delete the file
        // todo: Cleanup Job
        if (Links.Count != 0)
            throw new InvalidOperationException();
        
        Status = MediaStatus.Deleting;
    }

    public void MarkDeleted()
    {
        Status = MediaStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
    }

    // public Result AddLink(
    //     string ownerType,
    //     Guid ownerId,
    //     AttachmentMode mode)
    // {
    //     
    // }

    public bool IsDeleted() => Status == MediaStatus.Deleted;
}