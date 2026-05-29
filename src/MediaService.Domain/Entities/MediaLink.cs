namespace MediaService.Domain.Entities;

public sealed class MediaLink
{
    public Guid Id { get; private set; }

    public Guid MediaId { get; private set; }

    public string OwnerType { get; private set; } = null!;

    public Guid OwnerId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public MediaItem Media { get; private set; } = default!;

    private MediaLink()
    {
    }

    public MediaLink(
        Guid mediaId,
        string ownerType,
        Guid ownerId) : this()
    {
        Id = Guid.NewGuid();

        MediaId = mediaId;

        OwnerType = ownerType;

        OwnerId = ownerId;

        CreatedAt = DateTime.UtcNow;
    }
}