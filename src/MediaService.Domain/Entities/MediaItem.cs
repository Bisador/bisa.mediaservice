using MediaService.Domain.Enums;

namespace MediaService.Domain.Entities;

public class MediaItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string OriginalFileName { get; private set; } = null!;
    public string StoredFileName { get; private set; } = null!;
    
    public string BucketName { get; private set; }
    public string ObjectKey { get; private set; } = null!;
    
    public string ContentType { get; private set; } = null!;
    
    public long Size { get; private set; }
 
    public string? Sha256 { get; private set; }
    
    public string StorageProviderName { get; set; }
    
    public MediaStatus Status { get; set; }
    
    public string? OwnerId { get; private set; }

    public string? Metadata { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; private set; }

    private MediaItem()
    {
    }

    public MediaItem(
        string originalFileName,
        string storedFileName,
        string contentType,
        long size,
        string storagePath,
        string? ownerId) : this()
    {
        OriginalFileName = originalFileName;
        StoredFileName = storedFileName;
        ContentType = contentType;
        Size = size;
        StoragePath = storagePath;
        OwnerId = ownerId;
    }

    public void MarkDeleted()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }
}

