using MediaService.Domain.Enums;

namespace MediaService.Application.DTOs;

public sealed record MediaResponse(
    Guid Id,
    string Category,
    MediaAccessLevel AccessLevel,
    string OriginalFileName,
    string ContentType,
    long Size,
    string Url,
    DateTimeOffset CreatedAt);