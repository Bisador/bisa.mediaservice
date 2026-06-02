using MediaService.Domain.Enums;

namespace MediaService.Application.DTOs;

public sealed record MediaMetadataResponse
{
    public static MediaMetadataResponse NotExisted()
    {
        return new MediaMetadataResponse()
        {
            Exists = false
        };
    }

    public static MediaMetadataResponse Existed(
        Guid mediaId,
        Guid tenantId,
        bool isDeleted,
        bool isAvailable,
        string category)
    {
        return new MediaMetadataResponse()
        {
            Exists = true,
            MediaId = mediaId,
            TenantId = tenantId,
            IsDeleted = isDeleted,
            IsAvailable = isAvailable,
            Category = category,
        };
    }

    private MediaMetadataResponse()
    {
    }

    public bool Exists { get; init; }
    public Guid? MediaId { get; init; }
    public Guid? TenantId { get; init; }
    public bool? IsDeleted { get; init; }
    public bool? IsAvailable { get; init; }
    public MediaPurpose? MediaPurpose { get; init; }
    public string? Category { get; init; }
}