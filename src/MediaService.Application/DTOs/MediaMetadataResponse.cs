using MediaService.Domain.Enums;

namespace MediaService.Application.DTOs;

public sealed record MediaMetadataResponse
{
    public static MediaMetadataResponse NotExisted(Guid mediaId)
    {
        return new MediaMetadataResponse()
        {
            Id = mediaId,
            Exists = false
        };
    }

    public static MediaMetadataResponse Existed(
        Guid mediaId,
        Guid tenantId,
        MediaStatus status,
        string contentType,
        MediaAccessLevel accessLevel,
        string category,
        long size)
    {
        return new MediaMetadataResponse()
        {
            Exists = true,
            Id = mediaId,
            TenantId = tenantId,
            Status = status,
            ContentType = contentType,
            AccessLevel = accessLevel,
            Category = category,
            Size = size,
        };
    }

    public bool Exists { get; init; }
    public Guid Id { get; init; }
    public Guid? TenantId { get; init; }
    public MediaStatus Status { get; set; }
    public string? ContentType { get; set; }
    public MediaAccessLevel AccessLevel { get; set; }
    public MediaPurpose? MediaPurpose { get; init; }
    public long? Size { get; init; }
    public string? Category { get; init; }
}