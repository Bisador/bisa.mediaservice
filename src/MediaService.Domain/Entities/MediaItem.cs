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
    public string? OwnerId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; private set; }
    public string? Sha256 { get; private set; }


    public MediaItem(
        string originalFileName,
        string bucketName,
        string objectKey,
        string contentType,
        long size,
        string storageProvider,
        string? ownerId,
        string? sha256 = null) : this()
    {
        Id = Guid.NewGuid();

        OriginalFileName = originalFileName;

        BucketName = bucketName;
        ObjectKey = objectKey;

        ContentType = contentType;
        Size = size;

        StorageProvider = storageProvider;

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
        Status = MediaStatus.Deleting;
    }

    public void MarkDeleted()
    {
        Status = MediaStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
    }
    
 
    public bool IsDeleted() => Status == MediaStatus.Deleted;
}