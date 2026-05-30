namespace MediaService.Domain.Entities;

public sealed class MediaLink : IEquatable<MediaLink>
{
    public Guid Id { get; private set; } = Guid.NewGuid();
 
    public string OwnerType { get; private set; } = null!; 
    public string OwnerId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public MediaItem Media { get; private set; } = default!;
    public string MediaId { get; private set; }

    private MediaLink()
    {
    }

    public MediaLink( 
        string ownerType,
        string ownerId) : this()
    { 
        OwnerType = ownerType; 
        OwnerId = ownerId;

        CreatedAt = DateTime.UtcNow;
    }

    public bool Equals(MediaLink? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return OwnerType == other.OwnerType && OwnerId.Equals(other.OwnerId);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MediaLink other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(OwnerType, OwnerId);
    }
}